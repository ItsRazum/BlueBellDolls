namespace BlueBellDolls.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        #region Methods

        IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IEntityRepository<IEntity> GetRepository(Type entityType);

        #endregion
    }
}
