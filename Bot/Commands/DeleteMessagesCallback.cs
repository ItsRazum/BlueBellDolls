using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using Newtonsoft.Json;

namespace BlueBellDolls.Bot.Commands
{
    public class DeleteMessagesCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IMessagesProvider _messagesProvider;

        public DeleteMessagesCallback(
            IBotService botService,
            IMessagesProvider messagesProvider) 
            : base(botService)
        {
            _messagesProvider = messagesProvider;

            Handlers.Add("dm", HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split('-');
            var messageIds = JsonConvert.DeserializeObject<int[]>(args.Last());

            if (messageIds == null)
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateCouldNotExtractMessagesFromCallbackMessage(c), 
                    token: token);

                return;
            }


            var deleted = await BotService.DeleteMessagesAsync(c.Chat, messageIds, token);

            if (!deleted)
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateMessagesDeletingError(), token: token);
        }
    }
}
