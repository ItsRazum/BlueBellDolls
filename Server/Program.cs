using BlueBellDolls.Data.Contexts;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Factory;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Services;
using BlueBellDolls.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
        // Контроллеры и OpenAPI
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Конфигурация
        builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection(nameof(FileStorageSettings)));

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
            options.AddPolicy("AllowVueApp", policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        // Фабрики
        builder.Services
            .AddSingleton<IEntityFactory, EntityFactory>();

        //Остальные сервисы
        builder.Services
            .AddScoped<ILitterService, LitterService>()
            .AddScoped<IParentCatService, ParentCatService>()
            .AddScoped<IKittenService, KittenService>();
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
        app.UseDefaultFiles();
        app.MapStaticAssets();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AllowVueApp");
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapFallbackToFile("/index.html");
    }

    private static void InitializeDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        database.Database.Migrate();
    }

}