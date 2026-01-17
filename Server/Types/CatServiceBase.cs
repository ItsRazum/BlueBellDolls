using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Records;
using BlueBellDolls.Server.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Types
{
    public abstract class CatServiceBase<TEntity, TDto>(
        IWebHostEnvironment env, 
        IApplicationDbContext applicationDbContext,
        IOptions<FileStorageSettings> fileStorageSettings,
        ILogger logger) : DisplayableEntityServiceBase<TEntity, TDto>(env, applicationDbContext, fileStorageSettings, logger) where TEntity : Cat where TDto : class
    {
        public async Task<ServiceResult<TDto>> UpdateColorAsync(int id, string color, CancellationToken token = default)
        {
            var entity = await GetDetailEntityAsync(id, token);

            if (entity == null)
                return new(StatusCodes.Status404NotFound);

            entity.Color = color;
            await ApplicationDbContext.SaveChangesAsync(token);
            return new(StatusCodes.Status200OK, null, ModelToDtoFunc(entity));
        }
    }
}
