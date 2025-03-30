using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Services
{
    public class MessagesHelperService : IMessagesHelperService
    {
        private readonly IBotService _botService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService;
        private readonly CallbackDataSettings _callbackDataSettings;

        public MessagesHelperService(
            IBotService botService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            ICallbackDataProvider callbackDataProvider,
            IArgumentParseHelperService argumentParseHelperService,
            IOptions<BotSettings> options)
        {
            _botService = botService;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _callbackDataProvider = callbackDataProvider;
            _argumentParseHelperService = argumentParseHelperService;
            _callbackDataSettings = options.Value.CallbackDataSettings;
        }

        #region Photo Management

        public Task SendPhotoManagementMessageAsync(Chat chat, IDisplayableEntity entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosManagementMode.Photos, entity.Photos, token);

        public Task SendTitlesManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosManagementMode.Titles, entity.Titles, token);

        public Task SendGeneticTestsManagementMessageAsync(Chat chat, ParentCat entity, CancellationToken token = default)
            => SendManagementMessageAsync(chat, entity, PhotosManagementMode.GenTests, entity.GeneticTests, token);

        private async Task SendManagementMessageAsync(Chat chat, IDisplayableEntity entity, PhotosManagementMode photosManagementMode, IEnumerable<KeyValuePair<string, string>> photos, CancellationToken token = default)
        {
            var message = _messagesProvider.CreateEntityPhotosGuideMessage(entity, photosManagementMode);
            var inputFiles = photos.Select(p => new InputMediaPhoto(new InputFileId(p.Key))).ToArray();

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
            => await SendDeleteConfirmation(c, entity, PhotosManagementMode.Photos, entity.Photos, token);

        public async Task SendDeleteTitlesConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default)
            => await SendDeleteConfirmation(c, entity, PhotosManagementMode.Titles, entity.Titles, token);

        public async Task SendDeleteGeneticTestsConfirmationAsync(CallbackQueryAdapter c, ParentCat entity, CancellationToken token = default)
            => await SendDeleteConfirmation(c, entity, PhotosManagementMode.GenTests, entity.GeneticTests, token);

        private async Task SendDeleteConfirmation(CallbackQueryAdapter c, IDisplayableEntity entity, PhotosManagementMode photosManagementMode, Dictionary<string, string> photos, CancellationToken token)
        {

            var args = c.CallbackData.Split(_callbackDataSettings.ArgsSeparator); //[0]Command, [1]Entity Id

            var key = c.MessageText.Split('\n').Last();

            var (photoIndexes, photoMessageIds) = _argumentParseHelperService.ParsePhotosArgs(key);

            await _botService.DeleteMessagesAsync(c.Chat, [.. photoMessageIds, c.MessageId], token);

            var photoIds = new List<string>();
            foreach (var index in photoIndexes)
                photoIds.Add(photos.Keys.ToArray()[index]);

            var sendedMessages = await _botService.SendMessageAsync(
                c.Chat,
                _messagesProvider.CreateSelectedPhotosOverviewMessage(entity, photoIndexes.Count()),
                inputFiles: [.. photoIds.Select(p => new InputMediaPhoto(p))],
                token: token);

            int[] sendedMessageIds = [.. sendedMessages.Select(m => m.Id)];

            string redirectToCallback = photoIndexes.Count() == photos.Count
            ? _callbackDataProvider.CreateEditEntityCallback(entity)
            : _callbackDataProvider.CreateManagePhotosCallback(entity, photosManagementMode);

            var managePhotosCallback = _callbackDataProvider.CreateDeleteMessagesCallback() + _callbackDataSettings.MultipleCallbackSeparator + redirectToCallback;

            await _botService.SendMessageAsync(c.Chat,
                _messageParametersProvider.GetDeleteEntityPhotosConfirmationParameters(
                    entity,
                    c.CallbackData,
                    [.. photoIndexes],
                    [.. sendedMessages.Select(p => p.MessageId)],
                    managePhotosCallback,
                    managePhotosCallback),
                token);
        }

        #endregion
    }
}
