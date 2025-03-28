using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlueBellDolls.Bot.Commands
{
    public class DeleteMessagesCallback : CallbackHandler
    {
        private readonly IMessagesProvider _messagesProvider;
        private readonly IArgumentParseHelperService _argumentParseHelperService;

        public DeleteMessagesCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IArgumentParseHelperService argumentParseHelperService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _argumentParseHelperService = argumentParseHelperService;

            AddCommandHandler(CallbackDataProvider.GetDeleteMessagesCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            IEnumerable<int>? messageIds;
            if (c.CallbackData.Contains('-'))
            {
                var args = c.CallbackData.Split(CallbackArgsSeparator);
                messageIds = JsonConvert.DeserializeObject <int[]>(args.Last());
            }

            else
            {
                (_, messageIds) = _argumentParseHelperService.ParsePhotosArgs(c.MessageText);
            }


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
