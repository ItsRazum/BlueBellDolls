using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class SelectParentCatCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IEntityHelperService _entityHelperService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public SelectParentCatCallback(
            IBotService botService,
            IEntityHelperService entityHelperService,
            IMessageParametersProvider messageParametersProvider) 
            : base(botService)
        {
            _entityHelperService = entityHelperService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("selectParentCat", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split('-'); // [0]Command, [1]IsMale, [2]LitterId

            var isMale = bool.Parse(args[1]);
            var litterId = int.Parse(args[2]);

            var litter = await _entityHelperService.GetDisplayableEntityByIdAsync<Litter>(litterId, token);

            if (litter == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, $"Не удалось найти {nameof(Litter)} {litterId}!", token: token);
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
