using BlueBellDolls.Common.Interfaces;
using System.Linq.Expressions;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IEntityHelperService
    {
        Task<TEntity?> GetDisplayableEntityByIdAsync<TEntity>(
            int entityId, 
            CancellationToken token = default, 
            params Expression<Func<TEntity, object?>>[] includes)
            where TEntity : IDisplayableEntity;

        Task<(IEnumerable<TEntity> entityList, int pagesCount, int entitiesCount)> GetEntityListAsync<TEntity>(
            int page, 
            CancellationToken token = default)
            where TEntity : IDisplayableEntity;

        Task<(IEnumerable<TEntity> entityList, int pagesCount, int entitiesCount)> GetEntityListAsync<TEntity>(
            Expression<Func<TEntity, bool>> expression,
            int page,
            CancellationToken token = default)
            where TEntity : IDisplayableEntity;

        Task<TEntity> AddNewEntityAsync<TEntity>(CancellationToken token = default) 
            where TEntity : class, IDisplayableEntity, new();
    }
}
