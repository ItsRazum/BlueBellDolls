using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class EntityListCommand : CommandHandler<MessageAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public EntityListCommand(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("/catlist", HandleCommandAsync<ParentCat>);
            Handlers.Add("/litterlist", HandleCommandAsync<Litter>);
            Handlers.Add("/kittenlist", HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(MessageAdapter m, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var (entityList, pagesCount, entitiesCount) = await _entityHelperService.GetEntityListAsync<TEntity>(1, token);

            await BotService.SendMessageAsync(
                m.Chat,
                _messageParametersProvider.GetEntityListParameters(entityList, Enums.ListUnitActionMode.Edit, (1, pagesCount, entitiesCount)),
                token);
        }
    }
}
