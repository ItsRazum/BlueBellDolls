using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Litters
{
    public class AddKittenToLitterCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly ILitterManagementService _litterManagementService;
        private readonly IMessagesProvider _messagesProvider;

        public AddKittenToLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            ILitterManagementService managementService,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _litterManagementService = managementService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetAddKittenToLitterCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var litterId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var result = await _litterManagementService.AddNewKittenToLitterAsync(litterId, token);

            if (!result.Success && result.Value != null)
            {
                await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(result.Value), token);
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
                return;
            }
        }
    }
}
