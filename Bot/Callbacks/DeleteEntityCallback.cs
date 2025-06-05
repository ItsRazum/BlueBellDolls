using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class DeleteEntityCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private  readonly IMessagesProvider _messagesProvider;

        public DeleteEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
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
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

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
