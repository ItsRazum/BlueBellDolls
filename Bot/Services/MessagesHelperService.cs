using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Interfaces;
using Telegram.Bot.Types;

namespace BlueBellDolls.Bot.Services
{
    public class MessagesHelperService : IMessagesHelperService
    {
        private readonly IBotService _botService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public MessagesHelperService(
            IBotService botService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider)
        {
            _botService = botService;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
        }

        public async Task SendPhotoManagementMessageAsync(Chat chat, IDisplayableEntity entity, CancellationToken token = default)
        {
            var message = _messagesProvider.CreateEntityPhotosGuideMessage(entity);
            var inputFiles = entity.Photos
                    .Select(p => new InputMediaPhoto(new InputFileId(p.Key))).ToArray();

            var sendedPhotos = await _botService.SendMessageAsync(
                chat,
                message,
                inputFiles: inputFiles,
                token: token);

            await _botService.SendMessageAsync(
                chat,
                _messageParametersProvider.GetEntityPhotosParameters(
                    entity,
                    [],
                    [.. sendedPhotos.Select(m => m.MessageId)]),
                token);
        }
    }
}
