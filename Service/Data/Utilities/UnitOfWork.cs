using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Service.Data.Contexts;
using BlueBellDolls.Service.Interfaces;
using System.Collections.Concurrent;

namespace BlueBellDolls.Service.Data.Utilities
{
    internal class UnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly ApplicationDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        #endregion

        #region Constructor

        public UnitOfWork(
            ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region IUnitOfWork implementation

        public IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity
        {
            if (_repositories.TryGetValue(typeof(TEntity), out var repository))
            {
                return (IEntityRepository<TEntity>)repository;
            }

            var newRepository = _serviceProvider.GetRequiredService<IEntityRepository<TEntity>>();
            _repositories[typeof(TEntity)] = newRepository;

            return newRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        #endregion
    }
}
