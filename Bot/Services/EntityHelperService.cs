using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace BlueBellDolls.Bot.Services
{
    public class EntityHelperService : IEntityHelperService
    {
        private readonly IDatabaseService _databaseService;
        private readonly BotSettings _botSettings;

        public EntityHelperService(
            IDatabaseService databaseService,
            IOptions<BotSettings> botSettings)
        {
            _databaseService = databaseService;
            _botSettings = botSettings.Value;
        }

        public async Task<TEntity?> GetDisplayableEntityByIdAsync<TEntity>(
            int entityId, 
            CancellationToken token = default, 
            params Expression<Func<TEntity, object?>>[] includes) 
            where TEntity : IDisplayableEntity
        {
            return await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var repo = unit.GetRepository<TEntity>();
                var result = await repo.GetByIdAsync(entityId, ct, includes);
                return result;

            }, token);
        }

        public async Task<(IEnumerable<TEntity> entityList, int pagesCount, int entitiesCount)> GetEntityListAsync<TEntity>(
            int page, 
            CancellationToken token = default) 
            where TEntity : IDisplayableEntity
        {
            return await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var repo = unit.GetRepository<TEntity>();
                return (
                    await repo.GetByPageAsync(page, _botSettings.InlineKeyboardsSettings.PageSize, ct),
                    await repo.PagesCountAsync(_botSettings.InlineKeyboardsSettings.PageSize, token),
                    await repo.CountAsync(ct)
                );

            }, token);
        }

        public async Task<(IEnumerable<TEntity> entityList, int pagesCount, int entitiesCount)> GetEntityListAsync<TEntity>(
            Expression<Func<TEntity, bool>> expression,
            int page,
            CancellationToken token = default)
            where TEntity : IDisplayableEntity
        {
            return await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var repo = unit.GetRepository<TEntity>();
                return (
                    await repo.GetByPageAsync(expression, page, _botSettings.InlineKeyboardsSettings.PageSize, ct),
                    await repo.PagesCountAsync(_botSettings.InlineKeyboardsSettings.PageSize, token),
                    await repo.CountAsync(ct)
                );
            }, token);
        }

        public async Task<TEntity> AddNewEntityAsync<TEntity>(CancellationToken token = default) where TEntity : class, IDisplayableEntity, new()
        {
            return await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var result = new TEntity();
                await unit.GetRepository<TEntity>().AddAsync(result, ct);
                await unit.SaveChangesAsync(ct);
                return result;
            }, token);
        }
    }
}
