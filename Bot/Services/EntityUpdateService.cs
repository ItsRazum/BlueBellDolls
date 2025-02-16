using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Bot.Services
{
    public class EntityUpdateService : IEntityUpdateService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IEntityHelperService _entityHelperService;
        private readonly IEntityFormService _entityFormService;

        public EntityUpdateService(
            IDatabaseService databaseService,
            IEntityHelperService entityHelperService,
            IEntityFormService entityFormService)
        {
            _databaseService = databaseService;
            _entityHelperService = entityHelperService;
            _entityFormService = entityFormService;
        }

        public async Task<bool> HandleUpdateEntityByReplyAsync<TEntity>(
            MessageAdapter m,
            string modelName,
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token) where TEntity : class, IDisplayableEntity
        {
            var entity = await _entityHelperService.GetDisplayableEntityByIdAsync<TEntity>(modelId, token);
            return await _databaseService.ExecuteDbOperationAsync(async (unit, ct) =>
            {
                var entity = await unit.GetRepository<TEntity>().GetByIdAsync(modelId, ct);
                if (entity != null)
                {
                    foreach (var (propertyName, value) in properties)
                        if (!_entityFormService.UpdateProperty(entity, propertyName, value))
                            return false;

                    await unit.SaveChangesAsync(token);
                    return true;
                }
                return false;
            }, token);
        }
    }
}
