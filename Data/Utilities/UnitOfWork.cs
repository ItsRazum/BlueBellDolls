using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Data.Contexts;
using BlueBellDolls.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BlueBellDolls.Data.Utilities
{
    public class UnitOfWork(
        ApplicationDbContext dbContext,
        IServiceProvider serviceProvider) : IUnitOfWork, IDisposable
    {

        #region Fields

        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        #endregion

        #region IUnitOfWork implementation

        public IEntityRepository<TEntity> GetRepository<TEntity>() where TEntity : IEntity
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

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        #endregion
    }
}
