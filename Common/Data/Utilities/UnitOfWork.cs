using BlueBellDolls.Common.Data.Contexts;
using BlueBellDolls.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BlueBellDolls.Common.Data.Utilities
{
    public class UnitOfWork : IUnitOfWork
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
            if (!_repositories.TryGetValue(typeof(TEntity), out var repository))
            {
                var newRepository = _serviceProvider.GetRequiredService<IEntityRepository<TEntity>>();
                _repositories[typeof(TEntity)] = newRepository;

                return newRepository;
            }

            return (IEntityRepository<TEntity>)repository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public IEntityRepository<IEntity> GetRepository(Type entityType)
        {
            if (!_repositories.TryGetValue(entityType, out var repository))
            {
                var repositoryType = typeof(IEntityRepository<>).MakeGenericType(entityType);
                repository = _serviceProvider.GetRequiredService(repositoryType);
                _repositories[entityType] = repository;
            }

            return (IEntityRepository<IEntity>)repository;
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
