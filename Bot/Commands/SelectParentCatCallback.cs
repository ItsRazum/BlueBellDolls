using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class SelectParentCatCallback : CallbackHandler
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public SelectParentCatCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetSelectEntityCallback<ParentCat>(), HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]IsMale, [2]LitterId

            var isMale = bool.Parse(args[1]);
            var litterId = int.Parse(args[2]);

            var litter = await _entityHelperService.GetDisplayableEntityByIdAsync<Litter>(litterId, token);

            if (litter == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(typeof(Litter), litterId), token: token);
                return;
            }

            var (entityList, pagesCount, entitiesCount) = await _entityHelperService.GetEntityListAsync<ParentCat>(c => c.IsMale == isMale, 1, token);

            await BotService.EditMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetEntityListParameters(entityList, Enums.ListUnitActionMode.Select, (1, pagesCount, entitiesCount), litter),
                token);
        }
    }
}
