using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Providers;
using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.ValueConverters;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Contexts;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Data.Repositories;
using BlueBellDolls.Data.Repositories.Generic;
using BlueBellDolls.Data.Utilities;
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

        // ������������
        builder.Services
            .Configure<BotSettings>(builder.Configuration.GetSection(nameof(BotSettings)))
            .Configure<EntityFormSettings>(builder.Configuration.GetSection(nameof(EntityFormSettings)))
            .Configure<TelegramFilesHttpClientSettings>(builder.Configuration.GetSection(nameof(TelegramFilesHttpClientSettings)))
            .Configure<EntitySettings>(builder.Configuration.GetSection(nameof(EntitySettings)));

        // Entity Framework
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)), 
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly));
        });

        // �����������
        builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
        builder.Services.AddScoped<IEntityRepository<Litter>, LitterRepository>();

        // ����������
        builder.Services
            .AddSingleton<IMessagesProvider, MessagesProvider>()
            .AddSingleton<IKeyboardsProvider, KeyboardsProvider>()
            .AddSingleton<IMessageParametersProvider, MessageParametersProvider>()
            .AddSingleton<ICallbackDataProvider, CallbackDataProvider>();

        // �����
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // �������
        builder.Services.AddCommandHandlers();

        // HTTP �������
        builder.Services
            .AddHttpClient(builder.Configuration
                .GetSection(
                    nameof(TelegramFilesHttpClientSettings))[nameof(TelegramFilesHttpClientSettings.ClientName)] 
                    ?? throw new NullReferenceException(nameof(TelegramFilesHttpClientSettings.ClientName)))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                MaxConnectionsPerServer = int.Parse(builder.Configuration
                .GetSection(
                    nameof(TelegramFilesHttpClientSettings))
                    [nameof(TelegramFilesHttpClientSettings.MaxConnectionsPerServer)] 
                    ?? "50")
            });

        // Hosted �������
        builder.Services.AddHostedService<UpdateHandlerService>();

        // ��������� �������
        builder.Services
            .AddSingleton<IDatabaseService, DatabaseService>()
            .AddSingleton<IBotService, BotService>()
            .AddSingleton<IEntityFormService, EntityFormService>()
            .AddSingleton<IValueConverter, EntityValueConverter>()
            .AddSingleton<IEntityHelperService, EntityHelperService>()
            .AddSingleton<IArgumentParseHelperService, ArgumentParseHelperService>()
            .AddSingleton<IMessagesHelperService, MessagesHelperService>()
            .AddSingleton<IPhotosDownloaderService, PhotosDownloaderService>()
            .AddSingleton<IManagementService, ManagementService>();

        // ���. ���������
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
        dbContext.Database.Migrate();
    }
}