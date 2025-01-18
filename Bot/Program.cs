using BlueBellDolls.Bot.Services;
using BlueBellDolls.Bot.Settings;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddGrpc();

        builder.Services.AddSingleton<BotService>();

        var app = builder.Build();

        app.MapGrpcService<BlueBellDollsServiceClient>();

        app.Run();
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
}