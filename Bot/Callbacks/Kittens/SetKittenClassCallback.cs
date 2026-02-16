using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks.Kittens
{
    public class SetKittenClassCallback : CallbackHandler
    {

        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesProvider _managementServicesProvider;

        public SetKittenClassCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesProvider managementServicesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _managementServicesProvider = managementServicesProvider;

            AddCommandHandler(CallbackDataProvider.GetSetKittenClassCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var kittenId = int.Parse(args.Last());

            var kittenManagementService = _managementServicesProvider.GetKittenManagementService();

            if (Enum.TryParse<KittenClass>(args[1], out var kittenClass))
            {
                var result = await kittenManagementService.UpdateClassAsync(kittenId, kittenClass, token);
                if (result.Success && result.Value != null)
                {
                    await BotService.AnswerCallbackQueryAsync(
                        c.CallbackId,
                        _messagesProvider.CreateKittenClassSetSuccessMessage(result.Value),
                        token: token);

                    await BotService.EditOrSendNewMessageAsync(
                        c.Chat,
                        c.MessageId,
                        _messageParametersProvider.GetEntityFormParameters(result.Value),
                        token: token);
                }
                else
                {
                    await BotService.AnswerCallbackQueryAsync(
                        c.CallbackId,
                        _messagesProvider.CreateServerErrorMessage(result.Message),
                        token: token);
                }
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateServerErrorMessage(),
                    token: token);
            }
        }
    }
}
