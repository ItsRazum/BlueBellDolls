using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Enums;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class EntityListCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public EntityListCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<Litter>(), HandleCommandAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetListEntityCallback<Kitten>(), HandleCommandAsync<Kitten>);
        }

        public async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]EntityId
            var messageFirstRow = c.MessageText.Split('\n').FirstOrDefault();
            if (messageFirstRow != null) 
            {
                var listArgs = messageFirstRow.Split(' '); //[0]ListUnitActionType, [1]EntityType, [2]? "для" [3]? OwnerType, [4]? OwnerId
                if (!Enum.TryParse<ListUnitActionMode>(listArgs[0], out var actionType))
                    actionType = ListUnitActionMode.Edit;

                var ownerId = listArgs.Length == 4
                    ? int.Parse(listArgs[4]) : 0;

                var owner = await _entityHelperService.GetDisplayableEntityByIdAsync<Litter>(ownerId, token); //Могут быть проблемы, если владельцем в будущем сможет быть не только Litter

                var page = int.Parse(args.Last());
                var (entityList, pagesCount, entitiesCount) = await _entityHelperService.GetEntityListAsync<TEntity>(page, token);
                await BotService.EditMessageAsync(
                    c.Chat,
                    c.MessageId,
                    _messageParametersProvider.GetEntityListParameters(entityList, actionType, (page, pagesCount, entitiesCount), owner),
                    token);
            }

        }
    }
}
