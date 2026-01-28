using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Litters
{
    public class OpenEntityInLitterCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public OpenEntityInLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetOpenEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetOpenEntityCallback<Kitten>(), HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]ParentCatId, [2]LitterId
            var entityId = int.Parse(args.Last());
            var litterId = int.Parse(args[1]);

            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var result = await managementService.GetEntityAsync(entityId, token);

            if (!result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
                return;
            }

            await BotService.EditOrSendNewMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetEntityFromLitterParameters(result.Value!, litterId),
                token);
        }
    }
}
