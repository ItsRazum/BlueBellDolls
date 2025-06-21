using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class AddPhotosCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public AddPhotosCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IManagementService managementService)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementService = managementService;

            AddCommandHandler("фото", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.Photos == null) return;

            var repliedMessageText = m.ReplyToMessage!.Text;
            if (repliedMessageText == null) return;

            var messageArgs = repliedMessageText.Split('\n').First().Split(' ');
            var entityId = int.Parse(messageArgs[1]);
            var entityType = messageArgs.First();

            Func<PhotoAdapter[], int, CancellationToken, Task<ManagementOperationResult<IDisplayableEntity>>> addPhotosTask = entityType switch
            {
                "ParentCat" => _managementService.AddPhotosToEntity<ParentCat>,
                "Kitten" => _managementService.AddPhotosToEntity<Kitten>,
                "Litter" => _managementService.AddPhotosToEntity<Litter>,
                _ => throw new InvalidDataException(entityType)
            };

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLoadingMessage(), token: token);

            var result = await addPhotosTask(m.Photos, entityId, token);

            await BotService.DeleteMessageAsync(m.Chat, loadingMessage.Single().MessageId, token);
            if (result.Result != null)
            {
                if (!result.Success)
                    await BotService.SendMessageAsync(m.Chat, result.ErrorText!, token: token);
                else
                {
                    await BotService.DeleteMessagesAsync(m.Chat, [.. m.Photos.Select(p => p.MessageId), m.ReplyToMessage!.MessageId], token);
                    await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Result), token);
                }
            }
        }
    }
}