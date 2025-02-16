using BlueBellDolls.Common.Data.Contexts;
using BlueBellDolls.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlueBellDolls.Common.Types.Generic
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity
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
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await _dbSet.CountAsync(token);
        }

        public async Task<int> PagesCountAsync(int pageSize, CancellationToken token)
        {
            var count = await _dbSet.CountAsync(token);
            return (count + pageSize - 1) / pageSize;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token)
        {
            return await _dbSet.CountAsync(expression, token);
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken token)
        {
            var entity = await GetByIdAsync(id, token);
            
            if (entity != null)
                _dbSet.Remove(entity);

            return entity != null;
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken token, params Expression<Func<TEntity, object?>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => e.Id == id, token);
        }

        public async Task<IEnumerable<TEntity>> GetByPageAsync(int page, int pageSize, CancellationToken token)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<IEnumerable<TEntity>> GetByPageAsync(Expression<Func<TEntity, bool>> expression, int page, int pageSize, CancellationToken token)
        {
            return await _dbSet
                .Where(expression)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token, params Expression<Func<TEntity, object?>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach(var include in includes)
                query = query.Include(include);

            return await query.ToListAsync(token);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token, params Expression<Func<TEntity, object?>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(expression).ToListAsync(token);
        }

        public async Task UpdateAsync(TEntity entity, CancellationToken token)
        {
            var originalEntity = await GetByIdAsync(entity.Id, token) ?? throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");
            _dbContext.Entry(originalEntity).CurrentValues.SetValues(entity);
        }

        #endregion
    }
}
