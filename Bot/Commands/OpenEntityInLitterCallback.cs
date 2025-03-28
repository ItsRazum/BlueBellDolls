using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Commands
{
    public class OpenEntityInLitterCallback : CallbackHandler
    {
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IDatabaseService _databaseService;
        private readonly IMessagesProvider _messagesProvider;

        public OpenEntityInLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IMessageParametersProvider messageParametersProvider,
            IDatabaseService databaseService,
            IMessagesProvider messagesProvider) 
            : base(botService, botSettings, callbackDataProvider)
        {
            _messageParametersProvider = messageParametersProvider;
            _databaseService = databaseService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetOpenEntityCallback<ParentCat>(), HandleCommandAsync<ParentCat>);
            AddCommandHandler(CallbackDataProvider.GetOpenEntityCallback<Kitten>(), HandleCommandAsync<Kitten>);
        }

        private async Task HandleCommandAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : IDisplayableEntity
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); //[0]Command, [1]ParentCatId, [2]LitterId
            var entityId = int.Parse(args.Last());
            var litterId = int.Parse(args[1]);
            

            var entity = await _databaseService.ExecuteDbOperationAsync(
                async (unit, ct) => await unit.GetRepository<TEntity>().GetByIdAsync(entityId, ct),
                token);

            if (entity == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(), token: token);
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
