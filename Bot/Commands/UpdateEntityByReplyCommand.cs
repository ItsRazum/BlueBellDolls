using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class UpdateEntityByReplyCommand : CommandHandler<MessageAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IEntityUpdateService _entityUpdateService;

        public UpdateEntityByReplyCommand(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            IEntityUpdateService entityUpdateService)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _entityUpdateService = entityUpdateService;

            Handlers.Add("updateEntityByReply-ParentCat", HandleCommandAsync<ParentCat>);
            Handlers.Add("updateEntityByReply-Litter", HandleCommandAsync<Litter>);
            Handlers.Add("updateEntityByReply-Kitten", HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            if (m.ReplyToMessage != null && m.ReplyToMessage.Text != null)
            {
                var args = m.ReplyToMessage.Text.Split('\n');

                if (IsValidMessageFormat(args.First(), out var result))
                {
                    var properties = m.Text.Split('\n')
                        .Select(line => line.Split(": ", 2))
                        .Where(parts => parts.Length == 2)
                        .ToDictionary(parts => parts[0], parts => parts[1]);

                    if (properties == null || properties.Count == 0)
                        return;

                    var success = await _entityUpdateService.HandleUpdateEntityByReplyAsync<TEntity>(m, result.modelName, result.modelId, properties, token);

                    if (success)
                    {
                        if (!await BotService.DeleteMessageAsync(m.Chat, m.ReplyToMessage!.Id, token))
                        {
                            await BotService.EditMessageAsync(m.Chat, m.ReplyToMessage!.Id,
                                "Модель успешно обновлена!", token: token);
                        }

                        await BotService.DeleteMessageAsync(m.Chat, m.MessageId, token);
                        var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(result.modelId, token);

                        await BotService.SendMessageAsync(m.Chat,
                            _messageParametersProvider.GetEntityMessageParameters(entity!), token);
                    }
                    else
                        await BotService.SendMessageAsync(m.Chat, "Не удалось обновить модель! Возможно, была допущена ошибка при вводе значений.", token: token);
                }

                static bool IsValidMessageFormat(string message, out (string modelName, int modelId) values)
                {
                    values = (string.Empty, 0);

                    var parts = message.Split(' ');
                    if (parts.Length < 2)
                        return false;

                    values.modelName = parts[0];
                    if (!int.TryParse(parts[1], out values.modelId))
                        return false;

                    return true;
                }
            }
        }
    }
}
