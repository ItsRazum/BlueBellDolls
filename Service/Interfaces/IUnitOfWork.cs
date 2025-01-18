using BlueBellDolls.Common.Interfaces;

namespace BlueBellDolls.Service.Interfaces
{
    internal interface IUnitOfWork : IDisposable
    {

        #region Methods

        IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
