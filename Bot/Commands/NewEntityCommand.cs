using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class NewEntityCommand : CommandHandler<MessageAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public NewEntityCommand(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("/newcat", HandleCommandAsync<ParentCat>);
            Handlers.Add("/newlitter", HandleCommandAsync<Litter>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity, new()
        {
            var newEntity = await _entityHelperService.AddNewEntityAsync<TEntity>(token);
            await BotService.SendMessageAsync(m.Chat, _messageParametersProvider.GetEntityFormParameters(newEntity), token);
        }
    }
}
