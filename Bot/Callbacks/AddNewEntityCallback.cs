using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class AddNewEntityCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementService _managementService;

        public AddNewEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementService managementService)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _managementService = managementService;

            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<Litter>(), HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var result = await _managementService.AddNewEntityAsync<TEntity>(token);

            if (result.Result != null)
                await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(result.Result), token);

            else
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
        }
    }
}
