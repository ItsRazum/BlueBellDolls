using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Booking
{
    public class ClearBookingRequestKeyboardCallback : CallbackHandler
    {
        private readonly ICallbackDataProvider _callbackDataProvider;
        private readonly IMessagesProvider _messagesProvider;

        public ClearBookingRequestKeyboardCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings, 
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _callbackDataProvider = callbackDataProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(_callbackDataProvider.GetClearBookingRequestKeyboardCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var messageRows = c.MessageText.Split('\n').ToArray();
            messageRows[^1] = _messagesProvider.CreateBookingClosedWithoutKittenStatusChange();
            var messageText = string.Join('\n', messageRows);

            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, messageText, token: token);
        }
    }
}
