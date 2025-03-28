using BlueBellDolls.Bot.Types;

namespace BlueBellDolls.Bot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<CommandHandler>()
                .AddClasses(classes => classes.AssignableTo<CommandHandler>())
                .As<CommandHandler>()
                .WithTransientLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<CallbackHandler>()
                .AddClasses(classes => classes.AssignableTo<CallbackHandler>())
                .As<CallbackHandler>()
                .WithTransientLifetime());

            return services;
        }
    }
}
