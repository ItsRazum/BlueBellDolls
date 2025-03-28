using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class AddNewEntityCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public AddNewEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetAddEntityCallback<Litter>(), HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var newEntity = await _entityHelperService.AddNewEntityAsync<TEntity>(token);
            await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(newEntity), token);
        }
    }
}
