using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class DeleteEntityCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider;

        public DeleteEntityCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            ICallbackDataProvider callbackDataProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _callbackDataProvider = callbackDataProvider;

            Handlers.Add("deleteParentCat", HandleCallbackAsync<ParentCat>);
            Handlers.Add("deleteLitter", HandleCallbackAsync<Litter>);
            Handlers.Add("deleteKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split('-');
            var entityId = int.Parse(args.Last());
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Запрашиваемая сущность не найдена!", token: token);
                return;
            }

            var litterId = int.Parse(args[2]);

            (string onDeletionCanceledCallback, string rootPageCallback) = args.Contains("fromLitter")
                ? (_callbackDataProvider.CreateOpenEntityInLitterCallback(entity, litterId), _callbackDataProvider.CreateBackToLitterCallback(litterId))
                : (_callbackDataProvider.CreateEditEntityCallback(entity), _callbackDataProvider.CreateEditEntityCallback(entity));

            await BotService.EditMessageAsync(
                c.Chat, 
                c.MessageId,
                _messageParametersProvider.GetDeleteEntityConfirmationParameters(entity, c.CallbackData, onDeletionCanceledCallback, rootPageCallback), token);
        }
    }
}
