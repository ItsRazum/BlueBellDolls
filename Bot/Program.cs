using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Providers;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.ValueConverters;
using BlueBellDolls.Common.Data.CommandInterceptors;
using BlueBellDolls.Common.Data.Contexts;
using BlueBellDolls.Common.Data.Utilities;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types.Generic;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        SQLitePCL.Batteries.Init();

        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogging(builder);
        ConfigureServices(builder);
        ConfigureKestrel(builder);

        var app = builder.Build();

        ConfigureMiddleware(app);
        InitializeDatabase(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // gRPC
        builder.Services.AddGrpc();

        // Конфигурация
        builder.Services.Configure<BotSettings>(builder.Configuration.GetSection(nameof(BotSettings)));
        builder.Services.Configure<EntityFormSettings>(builder.Configuration.GetSection(nameof(EntityFormSettings)));

        // Entity Framework
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)));
            options.AddInterceptors(new SqliteForeignKeyEnforcer());
        });

        // Репозитории
        builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

        // Фабрики
        builder.Services.AddSingleton<IMessagesProvider, MessagesProvider>();
        builder.Services.AddSingleton<IKeyboardsProvider, KeyboardsProvider>();
        builder.Services.AddSingleton<IMessageParametersProvider, MessageParametersProvider>();
        builder.Services.AddSingleton<ICallbackDataProvider, CallbackDataProvider>();

        // Юниты
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Команды
        builder.Services.AddCommandHandlers();

        // Остальные сервисы
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IBotService, BotService>();
        builder.Services.AddSingleton<IUpdateHandlerService, UpdateHandlerService>();
        builder.Services.AddSingleton<IEntityFormService, EntityFormService>();
        builder.Services.AddSingleton<IValueConverter, EntityValueConverter>();
        builder.Services.AddSingleton<IEntityHelperService, EntityHelperService>();
        builder.Services.AddSingleton<IEntityUpdateService, EntityUpdateService>();

        // Бот
        builder.Services.AddHostedService<BotService>();

        // Доп. настройки
        builder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
    }

    private static void ConfigureKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel((context, options) =>
        {
            var grpcSettings = context.Configuration
                .GetSection(nameof(GrpcClientSettings))
                .Get<GrpcClientSettings>()
                ?? throw new NullReferenceException(nameof(GrpcClientSettings));

            options.Listen(IPAddress.Parse(grpcSettings.Host), grpcSettings.Port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
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

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.MapGrpcService<BlueBellDollsServiceClient>();
    }

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
}