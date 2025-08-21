using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        #region Methods

        IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
