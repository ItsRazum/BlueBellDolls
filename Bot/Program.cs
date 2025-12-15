using BlueBellDolls.Bot;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Factories;
using BlueBellDolls.Bot.Providers;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Services.Api;
using BlueBellDolls.Bot.Services.Management;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.ValueConverters;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using BlueBellDolls.Common.Services;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Bot.Interfaces.ValueConverters;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogging(builder);
        ConfigureServices(builder);

        var app = builder.Build();
        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Конфигурация
        builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);
        builder.Services
            .Configure<BotSettings>(builder.Configuration.GetSection(nameof(BotSettings)))
            .Configure<EntityFormSettings>(builder.Configuration.GetSection(nameof(EntityFormSettings)))
            .Configure<EntitySettings>(builder.Configuration.GetSection(nameof(EntitySettings)))
            .Configure<ApiSettings>(builder.Configuration.GetSection(nameof(ApiSettings)));

        // Провайдеры
        builder.Services
            .AddSingleton<IMessagesProvider, MessagesProvider>()
            .AddSingleton<IKeyboardsProvider, KeyboardsProvider>()
            .AddSingleton<IMessageParametersProvider, MessageParametersProvider>()
            .AddSingleton<ICallbackDataProvider, CallbackDataProvider>();

        // Команды
        builder.Services.AddCommandHandlers();

        // HTTP клиенты
        builder.Services.AddHttpClient(Constants.TelegramHttpClientName);
        builder.Services.AddHttpClient(Constants.BlueBellDollsHttpClientName, (services, client) =>
        {
            var config = services.GetRequiredService<IOptions<ApiSettings>>().Value;
            client.BaseAddress = new Uri(config.BaseUrl);
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config.JwtToken);
            client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
        });

        // Hosted сервисы
        builder.Services.AddHostedService<UpdateHandlerService>();

        // Клиент бота
        builder.Services.AddSingleton<ITelegramBotClient>(sp => 
            new TelegramBotClient(sp.GetRequiredService<IOptions<BotSettings>>().Value.Token));

        // Остальные сервисы
        builder.Services
            .AddSingleton<ICatColorTreeService, CatColorTreeService>()
            .AddSingleton<IBotService, BotService>()
            .AddSingleton<IEntityFormService, EntityFormService>()
            .AddSingleton<IValueConverter, EntityValueConverter>()
            .AddSingleton<IArgumentParseHelperService, ArgumentParseHelperService>()
            .AddSingleton<IMessagesHelperService, MessagesHelperService>()
            .AddSingleton<IPhotosDownloaderService, PhotosDownloaderService>()
            .AddSingleton<IEnumMapperService, EnumMapperService>()

            .AddScoped<IManagementServicesFactory, ManagementServicesFactory>()

            .AddScoped<IKittenApiClient, KittenApiClient>()
            .AddScoped<IParentCatApiClient, ParentCatApiClient>()
            .AddScoped<ILitterApiClient, LitterApiClient>()
            .AddScoped<ICatColorApiClient, CatColorApiClient>()
            .AddScoped<IBookingApiClient, BookingApiClient>()

            .AddScoped<IBookingManagementService, BookingManagementService>()

            .AddScoped<KittenManagementService>()
            .AddScoped<ICatManagementService<Kitten>>(s => s.GetRequiredService<KittenManagementService>())
            .AddScoped<IEntityManagementService<Kitten>>(s => s.GetRequiredService<KittenManagementService>())
            .AddScoped<IDisplayableEntityManagementService<Kitten>>(s => s.GetRequiredService<KittenManagementService>())

            .AddScoped<ParentCatManagementService>()
            .AddScoped<IParentCatManagementService>(s => s.GetRequiredService<ParentCatManagementService>())
            .AddScoped<ICatManagementService<ParentCat>>(s => s.GetRequiredService<ParentCatManagementService>())
            .AddScoped<IEntityManagementService<ParentCat>>(s => s.GetRequiredService<ParentCatManagementService>())
            .AddScoped<IDisplayableEntityManagementService<ParentCat>>(s => s.GetRequiredService<ParentCatManagementService>())

            .AddScoped<LitterManagementService>()
            .AddScoped<ILitterManagementService>(s => s.GetRequiredService<LitterManagementService>())
            .AddScoped<IEntityManagementService<Litter>>(s => s.GetRequiredService<LitterManagementService>())
            .AddScoped<IDisplayableEntityManagementService<Litter>>(s => s.GetRequiredService<LitterManagementService>())
            
            .AddScoped<CatColorManagementService>();

        // Доп. настройки
        builder.Services.AddMemoryCache();
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
    }
}