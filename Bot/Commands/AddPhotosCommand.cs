using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Commands
{
    public class AddPhotosCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesProvider _managementServicesProvider;

        public AddPhotosCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesProvider managementServicesProvider,
            IMessagesProvider messagesProvider)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementServicesProvider = managementServicesProvider;

            AddCommandHandler("фото", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(MessageAdapter m, CancellationToken token)
        {
            if (m.Photos == null) return;

            var repliedMessageText = m.ReplyToMessage!.Text;
            if (repliedMessageText == null) return;


            var messageArgs = repliedMessageText.Split('\n').First().Split(' ');
            var entityType = messageArgs.First();
            var entityId = int.Parse(messageArgs[1]);

            switch (entityType)
            {
                case "ParentCat":
                    await HandlePhotosAsync<ParentCat>(entityId, m, token);
                    break;
                case "Litter":
                    await HandlePhotosAsync<Litter>(entityId, m, token);
                    break;
                case "Kitten":
                    await HandlePhotosAsync<Kitten>(entityId, m, token);
                    break;
                case "CatColor":
                    await HandlePhotosAsync<CatColor>(entityId, m, token);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported entity type: {entityType}");
            }
        }

        private async Task HandlePhotosAsync<TEntity>(int entityId, MessageAdapter m, CancellationToken token = default) where TEntity : class, IDisplayableEntity
        {
            if (m.Photos == null) return;

            var repliedMessageText = m.ReplyToMessage?.Text;
            if (repliedMessageText == null) return;

            var loadingMessage = await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreatePhotosLoadingMessage(), token: token);
            var managementService = _managementServicesProvider.GetDisplayableEntityManagementService<TEntity>();

            var result = await managementService.AddPhotosToEntityAsync(entityId, m.Photos!, PhotosType.Photos, token);
            await BotService.DeleteMessageAsync(m.Chat, loadingMessage.Single().MessageId, token);

            if (result.Success)
            {
                await BotService.DeleteMessagesAsync(m.Chat, [.. m.Photos.Select(p => p.MessageId), m.ReplyToMessage!.MessageId], token);
                await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(result.Value!), token);
            }
            else
                await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateServerErrorMessage(result.Message), token: token);
        }
    }
}