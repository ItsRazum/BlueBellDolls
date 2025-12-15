using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Kittens
{
    public class OpenKittenClassCallback : CallbackHandler
    {

        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public OpenKittenClassCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IManagementServicesFactory managementServicesFactory,
            IMessageParametersProvider messageParametersProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetOpenKittenClassCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token = default)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var kittenId = int.Parse(args.Last());
            
            var kittenManagementService = _managementServicesFactory.GetEntityManagementService<Kitten>();
            var kitten = await kittenManagementService.GetEntityAsync(kittenId, token);

            if (kitten == null)
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateApiGetEntityFailureMessage(),
                    token: token);
                return;
            }

            await BotService.EditOrSendNewMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetKittenClassParameters(kitten),
                token: token);
        }
    }
}
