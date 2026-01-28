using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Providers;
using BlueBellDolls.Common.Services;
using BlueBellDolls.Data.Contexts;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Services;
using BlueBellDolls.Server.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using Telegram.Bot;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureLogging(builder);
        ConfigureServices(builder);

        var app = builder.Build();

        ConfigureServer(app);
        InitializeDatabase(app);

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        // Конфигурация
        builder.Configuration.AddJsonFile("appsettings.Secret.json", optional: true, reloadOnChange: true);
        builder.Services.Configure<TelegramNotificationSettings>(builder.Configuration.GetSection(nameof(TelegramNotificationSettings)));
        builder.Services.Configure<EntitiesSettings>(builder.Configuration.GetSection(nameof(EntitiesSettings)));
        builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(nameof(FileStorageSettings)));

        // Контроллеры и OpenAPI
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Rate limiting
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("BookingPolicy", options =>
            {
                options.PermitLimit = 5;
                options.Window = TimeSpan.FromMinutes(10);
                options.QueueLimit = 0;
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            });

            options.AddFixedWindowLimiter("FeedbackRequestPolicy", options =>
            {
                options.PermitLimit = 3;
                options.Window = TimeSpan.FromMinutes(30);
                options.QueueLimit = 0;
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        // Database
        builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly));
        });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowNuxtApp", policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
            };
        });

        //Вспомогательное
        builder.Services.AddHttpContextAccessor();

        // Остальные сервисы
        builder.Services
            .AddSingleton<ITelegramBotClient, TelegramBotClient>(sp =>
            {
                var botToken = builder.Configuration["TelegramNotificationSettings:BotToken"] ?? string.Empty;
                return new TelegramBotClient(botToken);
            })
            .AddSingleton<ICommonMessageParametersProvider, CommonMessageParametersProvider>()
            .AddSingleton<ICommonMessagesProvider, CommonMessagesProvider>()
            .AddSingleton<ICommonKeyboardsProvider, CommonKeyboardsProvider>()
            .AddSingleton<IBotService, BotService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<IFeedbackProcessingService, FeedbackProcessingService>()
            .AddScoped<ILitterService, LitterService>()
            .AddScoped<IParentCatService, ParentCatService>()
            .AddScoped<IKittenService, KittenService>()
            .AddScoped<ICatColorService, CatColorService>();
    }

    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
    }

    private static void ConfigureServer(WebApplication app)
    {
        if (!Directory.Exists("wwwroot"))
            Directory.CreateDirectory("wwwroot");

        var defaultCulture = new CultureInfo("ru-RU");

        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture),
            SupportedCultures = [defaultCulture],
            SupportedUICultures = [defaultCulture]
        };

        app.UseRequestLocalization(localizationOptions);
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapStaticAssets();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AllowNuxtApp");
        //app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        database.Database.Migrate();
    }
}