using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class UpdateEntityByReplyCommand : CommandHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementService _managementService;

        public UpdateEntityByReplyCommand(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider,
            IManagementService managementService)
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;
            _managementService = managementService;

            AddCommandHandler("update_entity_by_reply_parentcat", HandleCommandAsync<ParentCat>);
            AddCommandHandler("update_entity_by_reply_litter", HandleCommandAsync<Litter>);
            AddCommandHandler("update_entity_by_reply_kitten", HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            if (m.ReplyToMessage != null && m.ReplyToMessage.Text != null)
            {
                var args = m.ReplyToMessage.Text.Split('\n');

                if (IsValidMessageFormat(args.First(), out var modelId))
                {
                    var properties = m.Text.Split('\n')
                        .Select(line => line.Split(": ", 2))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(parts => parts[0], parts => parts[1]);

                    if (properties == null || properties.Count == 0)
                        return;

                    var result = await _managementService.UpdateEntityByReplyAsync<TEntity>(modelId, properties, token);

                    if (result.Success)
                    {
                        if (!await BotService.DeleteMessageAsync(m.Chat, m.ReplyToMessage!.MessageId, token))
                        {
                            await BotService.EditMessageAsync(m.Chat, m.ReplyToMessage!.MessageId,
                                _messagesProvider.CreateEntityUpdateSuccessMessage(), token: token);
                        }

                        await BotService.DeleteMessageAsync(m.Chat, m.MessageId, token);
                        var entity = result.Result;

                        await BotService.SendMessageAsync(m.Chat,
                            _messageParametersProvider.GetEntityFormParameters(entity!), token);
                    }
                    else
                        await BotService.SendMessageAsync(m.Chat, _messagesProvider.CreateEntityUpdateFailureMessage(), token: token);
                }

                static bool IsValidMessageFormat(string message, out int modelId)
                {
                    modelId = 0;

                    var parts = message.Split(' ');
                    if (parts.Length < 2)
                        return false;

                    if (!int.TryParse(parts[1], out modelId))
                        return false;

                    return true;
                }
            }
        }
    }
}
