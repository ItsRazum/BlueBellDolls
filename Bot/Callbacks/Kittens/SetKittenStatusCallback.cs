using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Callbacks.Kittens
{
    public class SetKittenStatusCallback : CallbackHandler
    {

        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public SetKittenStatusCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesFactory managementServicesFactory)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetSetKittenStatusCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator);
            var kittenId = int.Parse(args.Last());

            var kittenManagementService = _managementServicesFactory.GetKittenManagementService();

            if (Enum.TryParse<KittenStatus>(args[1], out var kittenStatus))
            {
                var result = await kittenManagementService.UpdateStatusAsync(kittenId, kittenStatus, token);

                if (result.Success)
                {
                    await BotService.AnswerCallbackQueryAsync(
                        c.CallbackId,
                        _messagesProvider.CreateKittenStatusSetSuccessMessage(result.Result!),
                        token: token);

                    await BotService.EditOrSendNewMessageAsync(
                        c.Chat,
                        c.MessageId,
                        _messageParametersProvider.GetEntityFormParameters(result.Result!),
                        token: token);
                }
                else
                {
                    await BotService.AnswerCallbackQueryAsync(
                        c.CallbackId,
                        _messagesProvider.CreateUnknownErrorMessage(result.ErrorText),
                        token: token);
                }
            }
            else
            {
                await BotService.AnswerCallbackQueryAsync(
                    c.CallbackId,
                    _messagesProvider.CreateUnknownErrorMessage(),
                    token: token);
            }
        }
    }
}
