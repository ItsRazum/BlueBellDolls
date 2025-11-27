using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Services
{
    public class MessagesHelperService(
        IBotService botService,
        IMessagesProvider messagesProvider,
        IMessageParametersProvider messageParametersProvider,
        ICallbackDataProvider callbackDataProvider,
        IArgumentParseHelperService argumentParseHelperService,
        IOptions<BotSettings> options) : IMessagesHelperService
    {

        #region Fields

        private readonly IBotService _botService = botService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider = messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider = callbackDataProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService = argumentParseHelperService;
        private readonly CallbackDataSettings _callbackDataSettings = options.Value.CallbackDataSettings;

        #endregion

        #region Photo Management

        public Task SendPhotoManagementMessageAsync(Chat chat, IDisplayableEntity entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosType.Photos, entity.Photos.Where(p => p.Type == PhotosType.Photos), token);

        public Task SendTitlesManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosType.Titles, entity.Photos.Where(p => p.Type == PhotosType.Titles), token);

        public Task SendGeneticTestsManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosType.GenTests, entity.Photos.Where(p => p.Type == PhotosType.GenTests), token);

        private async Task SendManagementMessageAsync(Chat chat, IDisplayableEntity entity, PhotosType photosManagementMode, IEnumerable<EntityPhoto> photos, CancellationToken token = default)
        {
            var message = _messagesProvider.CreateEntityPhotosGuideMessage(entity, photosManagementMode);
            var inputFiles = photos.Select(p => new InputMediaPhoto(new InputFileId(p.TelegramPhoto?.FileId ?? string.Empty))).ToArray();

            var sentPhotos = await _botService.SendMessageAsync(
                chat,
                message,
                inputFiles: inputFiles,
                token: token);

            await _botService.SendMessageAsync(
                chat,
                _messageParametersProvider.GetEntityPhotosParameters(
                    entity,
                    photosManagementMode,
                    [],
                    [.. sentPhotos.Select(m => m.MessageId)]),
                token);
        }

        #endregion

        #region Delete photos confirmation

        public async Task SendDeletePhotosConfirmationAsync(CallbackQueryAdapter c, IDisplayableEntity entity, CancellationToken token = default)
            => await SendDeleteConfirmation(c, entity, PhotosType.Photos, [.. entity.Photos.Where(p => p.Type == PhotosType.Photos)], token);

        public async Task SendDeleteTitlesConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default)
            => await SendDeleteConfirmation(c, entity, PhotosType.Titles, [.. entity.Photos.Where(p => p.Type == PhotosType.Titles)], token);

        public async Task SendDeleteGeneticTestsConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default)
            => await SendDeleteConfirmation(c, entity, PhotosType.GenTests, [.. entity.Photos.Where(p => p.Type == PhotosType.GenTests)], token);

        private async Task SendDeleteConfirmation(CallbackQueryAdapter c, IDisplayableEntity entity, PhotosType photosManagementMode, List<EntityPhoto> photos, CancellationToken token)
        {

            var args = c.CallbackData.Split(_callbackDataSettings.ArgsSeparator); //[0]Command, [1]Entity Id

            var key = c.MessageText.Split('\n').Last();

            var (photoKeys, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            await _botService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);

            var photoFileIds = photoKeys.Join(
                photos,
                pk => pk,
                p => p.Id,
                (pk, p) => new InputFileId(p.TelegramPhoto?.FileId ?? string.Empty));

            var sendedMessages = await _botService.SendMessageAsync(
                c.Chat,
                _messagesProvider.CreateSelectedPhotosOverviewMessage(entity, photoKeys.Count()),
                inputFiles: [.. photoFileIds.Select(p => new InputMediaPhoto(p))],
                token: token);

            int[] sendedMessageIds = [.. sendedMessages.Select(m => m.Id)];

            string redirectToCallback = photoKeys.Count() == photos.Count
            ? _callbackDataProvider.CreateEditEntityCallback(entity)
            : _callbackDataProvider.CreateManagePhotosCallback(entity, photosManagementMode);

            var managePhotosCallback = _callbackDataProvider.CreateDeleteMessagesCallback() + _callbackDataSettings.MultipleCallbackSeparator + redirectToCallback;

            await _botService.SendMessageAsync(c.Chat,
                _messageParametersProvider.GetDeleteEntityPhotosConfirmationParameters(
                    entity,
                    c.CallbackData,
                    [.. photoKeys],
                    [.. sendedMessages.Select(p => p.MessageId)],
                    managePhotosCallback,
                    managePhotosCallback),
                token);
        }

        #endregion
    }
}
