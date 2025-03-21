using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class SetDefaultPhotoCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessagesHelperService _messagesHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;

        public SetDefaultPhotoCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessagesHelperService messagesHelperService,
            IArgumentParseHelperService argumentParseHelperService) 
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messagesHelperService = messagesHelperService;
            _argumentParseHelperService = argumentParseHelperService;

            Handlers.Add("setDefaultPhotoForParentCat", HandleCallbackAsync<ParentCat>);
            Handlers.Add("setDefaultPhotoForLitter", HandleCallbackAsync<Litter>);
            Handlers.Add("setDefaultPhotoForKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) 
            where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split('-'); //[0]Command, [1]PhotoIndex, [2]Entity Id

            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Запрашиваемая сущность не найдена!", token: token);
                return;
            }

            var key = c.MessageText.Split('\n').Last();

            var (_, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            var photoIndex = int.Parse(args[1]);

            var photosList = entity.Photos.ToList();
            var photo = photosList.ElementAt(photoIndex);
            photosList.Remove(photo);
            entity.Photos = photosList.Prepend(photo).ToDictionary(x => x.Key, x => x.Value);

            await BotService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);

            await _messagesHelperService.SendPhotoManagementMessageAsync(c.Chat, entity, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackData, $"Фотография №{photoIndex + 1} успешно установлена как основная!", token: token);
        }
    }
}
