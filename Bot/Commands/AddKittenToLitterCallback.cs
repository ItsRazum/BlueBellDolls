using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class AddKittenToLitterCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public AddKittenToLitterCallback(
            IBotService botService,
            IDatabaseService databaseService,
            IMessageParametersProvider messageParametersProvider)
            : base(botService)
        {
            _databaseService = databaseService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("addKittenToLitter", HandleCallbackAsync);
        }

        private async Task HandleCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var litterId = int.Parse(c.CallbackData.Split('-').Last());

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
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Помёт не найден!", token: token);
                return;
            }

            await BotService.EditMessageAsync(c.Chat, c.MessageId, _messageParametersProvider.GetEntityMessageParameters(kitten), token);
        }
    }
}
