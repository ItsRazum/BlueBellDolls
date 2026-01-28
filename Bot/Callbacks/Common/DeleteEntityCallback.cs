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
            var result = await managementService.GetEntityAsync(entityId, token);

            if (!result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
                return;
            }

            var onDeletionCanceledCallback = CallbackDataProvider.CreateEditEntityCallback(result.Value!);

            if (args.Contains("fromLitter"))
            {
                var litterId = int.Parse(args[2]);
                onDeletionCanceledCallback = CallbackDataProvider.CreateOpenEntityInLitterCallback(result.Value!, litterId);
            }

            await BotService.EditOrSendNewMessageAsync(
                c.Chat, 
                c.MessageId,
                _messageParametersProvider.GetDeleteEntityConfirmationParameters(
                    result.Value!,
                    c.CallbackData,
                    onDeletionCanceledCallback), 
                token);
        }
    }
}
