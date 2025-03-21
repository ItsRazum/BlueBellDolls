using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Types.Generic;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Commands
{
    public class ConfirmDeleteEntityCallback : CommandHandler<CallbackQueryAdapter>
    {
        private readonly IDatabaseService _databaseService;

        public ConfirmDeleteEntityCallback(
            IBotService botService,
            IDatabaseService databaseService)
            : base(botService)
        {
            _databaseService = databaseService;

            Handlers.Add("confirm_deleteParentCat", HandleDeleteParentCatCallbackAsync);
            Handlers.Add("confirm_deleteLitter", HandleCallbackAsync<Litter>);
            Handlers.Add("confirm_deleteKitten", HandleCallbackAsync<Kitten>);
        }

        private async Task HandleCallbackAsync<TEntity>(CallbackQueryAdapter c, CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entityId = int.Parse(c.CallbackData.Split('-').Last());

            await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var repo = unit.GetRepository<TEntity>();

                if(await repo.DeleteByIdAsync(entityId, ct))
                    await unit.SaveChangesAsync(ct);
            }, token);

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Модель успешно удалена!", token: token);
        }

        private async Task HandleDeleteParentCatCallbackAsync(CallbackQueryAdapter c, CancellationToken token)
        {
            var entityId = int.Parse(c.CallbackData.Split('-').Last());

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

            await BotService.AnswerCallbackQueryAsync(c.CallbackId, "Модель успешно удалена!", token: token);
        }
    }
}
