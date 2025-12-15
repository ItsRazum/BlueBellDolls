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

        private async Task HandleCallbackAsync(CallbackQueryAdapter adapter, CancellationToken token)
        {

        }
    }
}
