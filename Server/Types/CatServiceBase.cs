using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Types
{
    public abstract class CatServiceBase<TEntity>(
        IWebHostEnvironment env, 
        IApplicationDbContext applicationDbContext, 
        ILogger logger) : DisplayableEntityServiceBase<TEntity>(env, applicationDbContext, logger) where TEntity : Cat
    {

        public async Task<ServiceResult> UpdateColorAsync(int id, string color, CancellationToken token = default)
        {
            var entity = await ApplicationDbContext.Set<TEntity>().FindAsync([id], token);
            if (entity == null) 
                return new(StatusCodes.Status404NotFound);

            entity.Color = color;
            await ApplicationDbContext.SaveChangesAsync(token);
            return new(StatusCodes.Status200OK);
        }
    }
}
