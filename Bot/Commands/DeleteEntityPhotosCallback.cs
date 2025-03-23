using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class DeleteEntityPhotosCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider;

        public DeleteEntityPhotosCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IArgumentParseHelperService argumentParseHelperService,
            IMessageParametersProvider messageParametersProvider,
            ICallbackDataProvider callbackDataProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _argumentParseHelperService = argumentParseHelperService;
            _messageParametersProvider = messageParametersProvider;
            _callbackDataProvider = callbackDataProvider;

            Handlers.Add("deletePhotosForParentCat", HandleCallbackAsync<ParentCat>);
            Handlers.Add("deletePhotosForLitter", HandleCallbackAsync<Litter>);
            Handlers.Add("deletePhotosForKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token)
            where TEntity : IDisplayableEntity
        {

            var args = c.CallbackData.Split('-'); //[0]Command, [2]Entity Id

            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Запрашиваемая сущность не найдена!", token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();

            var (photoIndexes, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            //await BotService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);

            var photoIds = new List<string>();
            foreach (var index in photoIndexes)
                photoIds.Add(entity.PhotoIds[index]);

            var managePhotosCallback = _callbackDataProvider.CreateAddPhotosCallback(entity);

            await BotService.SendMessageAsync(c.Chat, 
                _messageParametersProvider.GetDeleteEntityPhotosConfirmationParameters(
                    entity,
                    c.CallbackData, 
                    ([..photoIndexes], [..photoIds]),
                    managePhotosCallback, 
                    managePhotosCallback), 
                token);

        }
    }
}
