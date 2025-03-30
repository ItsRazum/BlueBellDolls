using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Callbacks
{
    public class ConfirmDeleteEntityCallback : CallbackHandler
    {
        private readonly IDatabaseService _databaseService;
        private readonly IMessagesProvider _messagesProvider;

        public ConfirmDeleteEntityCallback(
            IBotService botService,
            IOptions<BotSettings> botSettings,
            ICallbackDataProvider callbackDataProvider,
            IDatabaseService databaseService,
            IMessagesProvider messagesProvider)
            : base(botService, botSettings, callbackDataProvider)
        {
            _databaseService = databaseService;
            _messagesProvider = messagesProvider;

            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<ParentCat>(), HandleDeleteParentCatCallbackAsync);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Litter>(), HandleCallbackAsync<Litter>);
            AddCommandHandler(CallbackDataProvider.GetConfirmDeleteEntityCallback<Kitten>(), HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var repo = unit.GetRepository<TEntity>();

                if(await repo.DeleteByIdAsync(entityId, ct))
                    await unit.SaveChangesAsync(ct);
            }, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityDeletionSuccess(), token: token);
        }

        private async Task HandleDeleteParentCatCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var entityId = int.Parse(c.CallbackData.Split(CallbackArgsSeparator).Last());

            await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var parentCatRepo = unit.GetRepository<ParentCat>();
                var targetParentCat = await parentCatRepo.GetByIdAsync(entityId, ct);

                if (targetParentCat != null)
                {
                    var litterRepo = unit.GetRepository<Litter>();

                    IEnumerable<Litter> entities = targetParentCat.IsMale
                        ? await litterRepo.GetAllAsync(l => l.FatherCatId == targetParentCat.Id, ct, l => l.FatherCat)
                        : await litterRepo.GetAllAsync(l => l.MotherCatId == targetParentCat.Id, ct, l => l.MotherCat);

                    await parentCatRepo.DeleteByIdAsync(entityId, ct);
                    await unit.SaveChangesAsync(ct);
                }

            }, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, _messagesProvider.CreateEntityDeletionSuccess(), token: token);
        }
    }
}
