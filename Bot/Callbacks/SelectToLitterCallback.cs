using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace BlueBellDolls.Bot.Callbacks
{
    public class SelectToLitterCallback : CallbackHandler
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessageParametersProvider _messageParametersProvider;
        private readonly IMessagesProvider _messagesProvider;

        public SelectToLitterCallback(
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

            AddCommandHandler(CallbackDataProvider.GetSelectToLitterCallback(), HandleCommandAsync);
        }

        private async Task HandleCommandAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var args = c.CallbackData.Split(CallbackArgsSeparator); // [0]Command, [1]LitterId, [2]EntityType, [3]EntityId

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
                (var entityType, var entityId) = result.parentCat == null ? (typeof(ParentCat), parentCatId) : (typeof(Litter), litterId);
                await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityNotFoundMessage(entityType, entityId), token: token);
                return;
            }

            await BotService.AnswerCallbackQueryAsync(
                c.CallbackId,
                _messagesProvider.CreateParentCatSetForLitter(result.parentCat, result.litter),
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
