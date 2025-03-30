using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class AddKittenToLitterCallback : CallbackHandler
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public AddKittenToLitterCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IDatabaseService databaseService,
            IMessageParametersProvider messageParametersProvider,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _databaseService = databaseService;
            _messageParametersProvider = messageParametersProvider;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetAddKittenToLitterCallback(), HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var litterId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            var kitten = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var kitten = new Kitten();

                var litterRepo = unit.GetRepository<Litter>();

                var litter = await litterRepo.GetByIdAsync(litterId, ct, l => l.Kittens);

                if (litter == null)
                    return null;

                litter.Kittens.Add(kitten);
                await unit.SaveChangesAsync(token);

                return kitten;

            }, token);

            if (kitten == null)
            {
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(typeof(Litter), litterId), token: token);
                return;
            }

            await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityFormParameters(kitten), token);
        }
    }
}
