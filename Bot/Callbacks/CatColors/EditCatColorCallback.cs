using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Providers;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;
using CatColor = BlueBellDolls.Common.Models.CatColor;

namespace BlueBellDolls.Bot.Callbacks.CatColors
{
    public class EditCatColorCallback : CallbackHandler
    {
        private readonly ICatColorManagementService _catColorManagementService;
        private readonly IMessagesProvider _messagesProvider;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public EditCatColorCallback(
            IBotService botService, 
            IOptions<BotSettings> botSettings, 
            ICallbackDataProvider callbackDataProvider,
            ICatColorManagementService catColorManagementService,
            IMessagesProvider messagesProvider,
            IMessageParametersProvider messageParametersProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _catColorManagementService = catColorManagementService;
            _messagesProvider = messagesProvider;
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler(CallbackDataProvider.GetEditEntityCallback<CatColor>(), HandleCommandAsync);
        }


        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var searchQuery = c.CallbackData.Split(CallbackArgsSeparator).Last();
            var result = int.TryParse(searchQuery, out var entityId)
                ? await _catColorManagementService.GetEntityAsync(entityId, token)
                : await _catColorManagementService.GetEntityAsync(searchQuery, token);

            if (!result.Success)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateUnknownErrorMessage(result.Message), token: token);
                return;
            }

            await BotService.EditOrSendNewMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(result.Value!), token);
        }
    }
}
