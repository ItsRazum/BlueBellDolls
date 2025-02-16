using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Types.Generic;

namespace BlueBellDolls.Bot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<CommandHandler<MessageAdapter>>()
                .AddClasses(classes => classes.AssignableTo<CommandHandler<MessageAdapter>>())
                .As<CommandHandler<MessageAdapter>>()
                .WithTransientLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<CommandHandler<CallbackQueryAdapter>>()
                .AddClasses(classes => classes.AssignableTo<CommandHandler<CallbackQueryAdapter>>())
                .As<CommandHandler<CallbackQueryAdapter>>()
                .WithTransientLifetime());

            return services;
        }
    }
}
