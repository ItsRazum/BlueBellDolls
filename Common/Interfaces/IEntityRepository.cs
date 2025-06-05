using System.Linq.Expressions;

namespace BlueBellDolls.Common.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : IEntity
    {

        #region Methods

        Task AddAsync(TEntity entity, CancellationToken token);
        Task<bool> DeleteByIdAsync(int id, CancellationToken token);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken token);
        Task<IEnumerable<TEntity>> GetByPageAsync(int page, int pageSize, CancellationToken token);
        Task<IEnumerable<TEntity>> GetByPageAsync(Expression<Func<TEntity, bool>> expression, int page, int pageSize, CancellationToken token);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token, params Expression<Func<TEntity, object?>>[] includes);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token, params Expression<Func<TEntity, object?>>[] includes);
        Task<int> CountAsync(CancellationToken token);
        Task<int> PagesCountAsync(int pageSize, CancellationToken cancellationToken);
        Task<int> PagesCountAsync(Expression<Func<TEntity, bool>> expression, int pageSize, CancellationToken token);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token);

        #endregion

    }
}
