using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class DeleteEntityCallback : CallbackHandler
    {
        private readonly IManagementServicesFactory _managementServiceFactory;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public DeleteEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _managementServiceFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetDeleteEntityCallback<ParentCat>(), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetDeleteEntityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetDeleteEntityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var entityId = int.Parse(args.Last());
            var managementService = _managementServiceFactory.GetEntityManagementService<TEntity>();
            var entity = await managementService.GetEntityAsync(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
                return;
            }

            (string onDeletionCanceledCallback, string rootPageCallback) callbacks = (
                CallbackDataProvider.CreateEditEntityCallback(entity), 
                CallbackDataProvider.CreateListEntityCallback(entity.GetType().Name, 1));

            if (args.Contains("fromLitter"))
            {
                var litterId = int.Parse(args[2]);
                callbacks = (
                    CallbackDataProvider.CreateOpenEntityInLitterCallback(entity, litterId), 
                    CallbackDataProvider.CreateBackToLitterCallback(litterId));
            }

            await BotService.EditOrSendNewMessageAsync(
                c.Chat, 
                c.MessageId,
                _messageParametersProvider.GetDeleteEntityConfirmationParameters(
                    entity,
                    c.CallbackData, 
                    callbacks.onDeletionCanceledCallback, 
                    callbacks.rootPageCallback), 
                token);
        }
    }
}
