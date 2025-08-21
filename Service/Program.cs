using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Data.Contexts;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Data.Repositories.Generic;
using BlueBellDolls.Data.Utilities;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Services;
using BlueBellDolls.Service.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

namespace BlueBellDolls.Service;

class Program
{
    static void Main(string[] args)
    {
        SQLitePCL.Batteries.Init();

        var builder = WebApplication.CreateBuilder(args);

        #region Configure Application
        ConfigureLogging(builder);
        ConfigureServices(builder);
        ConfigureKestrel(builder);
        #endregion

        var app = builder.Build();

        #region Configure Middleware and Database
        ConfigureMiddleware(app);
        InitializeDatabase(app);
        #endregion

        app.Run();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // gRPC
        builder.Services.AddGrpc();

        // Entity Framework
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)),
                x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly));
        });

        // Репозитории
        builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

        // Настройки gRPC
        builder.Services.Configure<GrpcServerSettings>(builder.Configuration.GetSection(nameof(GrpcServerSettings)));

        // Юниты
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Сервисы
        builder.Services.AddScoped<ICatService, CatService>();

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
                .GetSection(nameof(GrpcServerSettings))
                .Get<GrpcServerSettings>()
                ?? throw new NullReferenceException(nameof(GrpcServerSettings));

            options.Listen(IPAddress.Parse(grpcSettings.Host), grpcSettings.Port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.MapGrpcService<BlueBellDollsService>().EnableGrpcWeb();
    }

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }
}
