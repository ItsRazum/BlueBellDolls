using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.AspNetCore.Components.Web;

namespace BlueBellDolls.Bot.Commands
{
    public class TogglePhotoSelectionCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService;

        public TogglePhotoSelectionCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            IArgumentParseHelperService argumentParseHelperService) 
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _argumentParseHelperService = argumentParseHelperService;

            Handlers.Add("togglePhotoForParentCat", HandleCallbackAsync<ParentCat>);
            Handlers.Add("togglePhotoForKitten", HandleCallbackAsync<Kitten>);
            Handlers.Add("togglePhotoForLitter", HandleCallbackAsync<Litter>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split('-'); //[0]Command, [1] PhotoIndex, [2]bool Select [3]EntityId
            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Не удалось найти целевую сущность!", token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();
            var selectedPhotoIndex = int.Parse(args[1]);

            var (photoIndexes, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            photoIndexes = bool.Parse(args[2]) 
                ? photoIndexes.Append(selectedPhotoIndex)
                : photoIndexes.Where(x => x != selectedPhotoIndex);

            await BotService.EditMessageAsync(
                c.Chat, 
                c.MessageId, 
                _messageParametersProvider.GetEntityPhotosParameters(
                    entity, 
                    [..photoIndexes.Order()], 
                    [..photoMessageIds]),
                token);
        }
    }
}
