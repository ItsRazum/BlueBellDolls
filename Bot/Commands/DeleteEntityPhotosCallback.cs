using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Commands
{
    public class DeleteEntityPhotosCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public DeleteEntityPhotosCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IArgumentParseHelperService argumentParseHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _argumentParseHelperService = argumentParseHelperService;
            _messageParametersProvider = messageParametersProvider;
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token)
            where TEntity : IDisplayableEntity
        {

            var args = c.CallbackData.Split('-'); //[0]Command, [1]int[] SelectedPhotos, [2]Entity Id

            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Запрашиваемая сущность не найдена!", token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();

            var (_, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            await BotService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);

        }
    }
}
