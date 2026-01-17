using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Booking
{
    public class SetBookingKittenStatusCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ICallbackDataProvider _callbackDataProvider;
        private readonly IBookingManagementService _bookingManagementService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public SetBookingKittenStatusCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IBookingManagementService bookingManagementService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesFactory managementServicesFactory)
            : base(botService, botSettings, callbackDataProvider)
        {
            _callbackDataProvider = callbackDataProvider;
            _bookingManagementService = bookingManagementService;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(_callbackDataProvider.GetSetBookingKittenStatusCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var kittenStatus = Enum.Parse<KittenStatus>(args[1]);
            var kittenId = int.Parse(args.Last());
            var kittenManagementService = _managementServicesFactory.GetKittenManagementService();
            var result = await kittenManagementService.UpdateStatusAsync(kittenId, kittenStatus, token);
            if (result.Success && result.Result != null)
            {
                await BotService.EditOrSendNewMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messagesProvider.CreateBookingKittenStatusChangedMessage(kittenStatus),
                    token: token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateApiUpdateEntityFailureMessage(),
                    true,
                    token);
            }
        }
    }
}
