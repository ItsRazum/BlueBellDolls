using System.Linq.Expressions;

namespace BlueBellDolls.Common.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class, IEntity
    {

        #region Methods

        Task AddAsync(TEntity entity, CancellationToken token);
        Task UpdateAsync(TEntity entity, CancellationToken token);
        Task DeleteAsync(TEntity entity, CancellationToken token);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken token);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token);
        Task<int> CountAsync(CancellationToken token);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token);

        #endregion

    }
}
