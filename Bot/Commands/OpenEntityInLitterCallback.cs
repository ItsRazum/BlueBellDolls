using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class OpenEntityInLitterCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IDatabaseService _databaseService;

        public OpenEntityInLitterCallback(
            IBotService botService,
            IMessageParametersProvider messageParametersProvider,
            IDatabaseService databaseService) 
            : base(botService)
        {
            _messageParametersProvider = messageParametersProvider;
            _databaseService = databaseService;

            Handlers.Add("openParentCat", HandleCommandAsync<ParentCat>);
            Handlers.Add("openKitten", HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split('-'); //[0]Command, [1]ParentCatId, [2]LitterId
            var entityId = int.Parse(args.Last());
            var litterId = int.Parse(args[1]);
            

            var entity = await _databaseService.ExecuteDbOperationAsync(
                async (unit, ct) => await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct),
                token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Не удалось найти сущность!", token: token);
                return;
            }

            await BotService.EditMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetEntityFromLitterParameters(entity, litterId),
                token);
        }
    }
}
