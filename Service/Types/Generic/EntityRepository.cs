using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Service.Data.Contexts;
using BlueBellDolls.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlueBellDolls.Service.Types.Generic
{
    internal class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity
    {
        #region Fields

        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        #endregion

        #region Constructor

        public EntityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        #endregion

        #region IEntityRepository implementation

        public async Task AddAsync(TEntity entity, CancellationToken token)
        {
            await _dbSet.AddAsync(entity, token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _dbSet.CountAsync(token);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token)
        {
            return await _dbSet.CountAsync(expression, token);
        }

        public async Task DeleteAsync(TEntity entity, CancellationToken token)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken token)
        {
            return await _dbSet.FindAsync([id], token);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token)
        {
            return await _dbSet.ToListAsync(token);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token)
        {
            return await _dbSet.Where(expression).ToListAsync(token);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken token)
        {
            var originalEntity = await GetByIdAsync(entity.Id, token) ?? throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");
            _dbContext.Entry(originalEntity).CurrentValues.SetValues(entity);
        }

        #endregion
    }
}
