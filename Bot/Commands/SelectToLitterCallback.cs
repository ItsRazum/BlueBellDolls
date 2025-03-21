using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Models;
using System.Globalization;

namespace BlueBellDolls.Bot.Commands
{
    public class SelectToLitterCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;

        public SelectToLitterCallback(
            IBotService botService,
            IDatabaseService databaseService,
            IMessageParametersProvider messageParametersProvider) : base(botService)
        {
            _databaseService = databaseService;
            _messageParametersProvider = messageParametersProvider;

            Handlers.Add("selectToLitter", HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split('-'); // [0]Command, [1]LitterId, [2]EntityType, [3]EntityId

            var litterId = int.Parse(args[1]);
            var parentCatId = int.Parse(args[3]);

            var result = await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var parentCatTask = unit.GetRepository<ParentCat>().GetByIdAsync(parentCatId, ct);
                var litterRepo = unit.GetRepository<Litter>();
                var litterTask = litterRepo.GetByIdAsync(litterId, ct, l => l.Kittens, l => l.FatherCat, l => l.MotherCat);

                await Task.WhenAll(parentCatTask, litterTask);

                var parentCat = await parentCatTask;
                var litter = await litterTask;

                if (parentCat == null || litter == null)
                    return (litter, parentCat);

                if (parentCat.IsMale)
                    litter.FatherCat = parentCat;
                else
                    litter.MotherCat = parentCat;

                await unit.SaveChangesAsync(ct);

                return (litter, parentCat);
            }, token);

            if (result.parentCat == null || result.litter == null)
            {
                var missingEntity = result.parentCat == null ? $"ParentCat {parentCatId}" : $"Litter {litterId}";
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, $"Ошибка: {missingEntity} не найден!", token: token);
                return;
            }

            var parentGender = result.parentCat.IsMale ? "папа" : "мама";
            await BotService.AnswerCallbackQueryAsync(
                c.CallbackId,
                $"Установлен родитель ({parentGender}) для помёта {result.litter.Letter} (от {result.litter.BirthDay.ToString("d", new CultureInfo("ru-RU"))})",
                token: token
            );

            await BotService.EditMessageAsync(
                c.Chat,
                c.MessageId,
                _messageParametersProvider.GetEntityFormParameters(result.litter),
                token
            );
        }
    }
}
