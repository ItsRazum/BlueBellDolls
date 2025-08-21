using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlueBellDolls.Data.Repositories.Generic
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity
    {
        #region Fields

        protected DbSet<TEntity> DbSet { get; }

        #endregion

        #region Constructor

        public EntityRepository(ApplicationDbContext dbContext)
        {
            DbSet = dbContext.Set<TEntity>();
        }

        #endregion

        #region IEntityRepository implementation

        public virtual async Task AddAsync(TEntity entity, CancellationToken token)
        {
            await DbSet.AddAsync(entity, token);
        }

        public virtual async Task<int> CountAsync(CancellationToken token)
        {
            return await DbSet.CountAsync(token);
        }

        public virtual async Task<int> PagesCountAsync(int pageSize, CancellationToken token)
        {
            var count = await DbSet.CountAsync(token);
            return (count + pageSize - 1) / pageSize;
        }

        public virtual async Task<int> PagesCountAsync(Expression<Func<TEntity, bool>> expression, int pageSize, CancellationToken token)
        {
            var count = await DbSet.CountAsync(expression, token);
            return (count + pageSize - 1) / pageSize;
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token)
        {
            return await DbSet.CountAsync(expression, token);
        }

        public virtual async Task<bool> DeleteByIdAsync(int id, CancellationToken token)
        {
            var entity = await GetByIdAsync(id, token);
            
            if (entity != null)
                DbSet.Remove(entity);

            return entity != null;
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken token)
        {
            return await DbSet.FirstOrDefaultAsync(e => e.Id == id, token);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByPageAsync(int page, int pageSize, CancellationToken token)
        {
            return await DbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public virtual async Task<IEnumerable<TEntity>> GetByPageAsync(Expression<Func<TEntity, bool>> expression, int page, int pageSize, CancellationToken token)
        {
            return await DbSet
                .Where(expression)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token, params Expression<Func<TEntity, object?>>[] includes)
        {
            IQueryable<TEntity> query = DbSet;

            foreach(var include in includes)
                query = query.Include(include);

            return await query.ToListAsync(token);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression, CancellationToken token, params Expression<Func<TEntity, object?>>[] includes)
        {
            IQueryable<TEntity> query = DbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(expression).ToListAsync(token);
        }

        #endregion
    }
}
