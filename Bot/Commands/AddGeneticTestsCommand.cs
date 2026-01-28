using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Commands
{
    public class AddGeneticTestsCommand : CommandHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public AddGeneticTestsCommand(
            IBotService botService,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _messagesProvider = messagesProvider;
            _managementServicesFactory = managementServicesFactory;
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler("генетический", HandleCommandAsync);
            AddCommandHandler("тесты", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.Photos == null) return;

            var repliedMessageText = m.ReplyToMessage?.Text;
            if (repliedMessageText == null) return;

            var messageArgs = repliedMessageText.Split('\n').First().Split(' ');
            var entityId = int.Parse(messageArgs[1]);

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLoadingMessage(), token: token);

            var managementService = _managementServicesFactory.GetParentCatManagementService();
            var result = await managementService.AddPhotosToEntityAsync(entityId, m.Photos, PhotosType.GenTests, token);

            await BotService.DeleteMessageAsync(m.Chat, loadingMessage.Single().MessageId, token);

            if (result.Success)
            {
                await BotService.DeleteMessagesAsync(m.Chat, [.. m.Photos.Select(p => p.MessageId), m.ReplyToMessage!.MessageId], token);
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Value!), token);
            }
            else
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
        }
    }
}
