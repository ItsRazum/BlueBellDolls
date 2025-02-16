using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class AddNewEntityCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public AddNewEntityCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("addParentCat", HandleCommandAsync<ParentCat>);
            Handlers.Add("addLitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var newEntity = await _entityHelperService.AddNewEntityAsync<TEntity>(token);
            await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityMessageParameters(newEntity), token);
        }
    }
}
