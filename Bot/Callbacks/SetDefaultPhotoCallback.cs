using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class SetDefaultPhotoCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessagesHelperService _messagesHelperService;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly IMessagesProvider _messagesProvider;

        public SetDefaultPhotoCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessagesHelperService messagesHelperService,
            IArgumentParseHelperService argumentParseHelperService,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messagesHelperService = messagesHelperService;
            _argumentParseHelperService = argumentParseHelperService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<ParentCat>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Litter>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetSetDefaultPhotoCallback<Kitten>(Enums.PhotosManagementMode.Photos), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) 
            where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]PhotoIndex, [2]Entity Id

            var entityId = int.Parse(args.Last());

            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(entityId, token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
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

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateDefaultPhotoSetForEntityMessage(entity, photoIndex), token: token);
        }
    }
}
