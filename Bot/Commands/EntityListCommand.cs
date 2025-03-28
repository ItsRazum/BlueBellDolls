using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class EntityListCommand : CommandHandler
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

            AddCommandHandler("/catlist", HandleCommandAsync<ParentCat>);
            AddCommandHandler("/litterlist", HandleCommandAsync<Litter>);
            AddCommandHandler("/kittenlist", HandleCommandAsync<Kitten>);
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
