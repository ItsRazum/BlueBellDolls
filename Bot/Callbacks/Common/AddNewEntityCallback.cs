using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using Microsoft.Extensions.Options;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Interfaces;
using CatColor = BlueBellDolls.Common.Models.CatColor;
using BlueBellDolls.Bot.Interfaces.Factories;
using BlueBellDolls.Bot.Interfaces.Providers;

namespace BlueBellDolls.Bot.Callbacks.Common
{
    public class AddNewEntityCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IManagementServicesFactory _managementServicesFactory;

        public AddNewEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IManagementServicesFactory managementServicesFactory)
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _managementServicesFactory = managementServicesFactory;

            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<Litter>(), HandleCommandAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<CatColor>(), HandleCommandAsync<CatColor>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var managementService = _managementServicesFactory.GetEntityManagementService<TEntity>();
            var result = await managementService.AddNewEntityAsync(token);

            if (result.Result != null)
                await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(result.Result), token);

            else
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, result.ErrorText!, token: token);
        }
    }
}
