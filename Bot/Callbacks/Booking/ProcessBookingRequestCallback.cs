using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Booking
{
    public class ProcessBookingRequestCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider;
        private readonly IBookingManagementService _bookingManagementService;

        public ProcessBookingRequestCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings, 
            ICallbackDataProvider callbackDataProvider,
            IBookingManagementService bookingManagementService) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _callbackDataProvider = callbackDataProvider;
            _bookingManagementService = bookingManagementService;

            AddCommandHandler(_callbackDataProvider.GetProcessBookingCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var bookingId = int.Parse(args.Last());
            var result = await _bookingManagementService.ProcessBookingRequestAsync(bookingId, c.From!.Id, token);
            if (result.Success)
            {
                await BotService.EditOrSendNewMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messageParametersProvider.GetBookingProcessingParameters(),
                    token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    $"Failed to process booking request: {result.ErrorText}",
                    true,
                    token);
            }
        }
    }
}
