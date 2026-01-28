using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Types
{
    public abstract class CatServiceBase<TEntity, TDto>(
        IWebHostEnvironment env, 
        IApplicationDbContext applicationDbContext,
        IOptions<FileStorageSettings> fileStorageSettings,
        ICatColorService catColorService,
        ILogger logger) : DisplayableEntityServiceBase<TEntity, TDto>(env, applicationDbContext, fileStorageSettings, logger) where TEntity : Cat where TDto : class
    {
        public async Task<ServiceResult<TDto>> UpdateColorAsync(int id, string color, CancellationToken token = default)
        {
            var entity = await GetDetailEntityAsync(id, token);

            if (entity == null)
                return new(StatusCodes.Status404NotFound);

            var catColorResult = await catColorService.GetAsync(true, color, token);

            if (catColorResult.StatusCode != StatusCodes.Status200OK)
                return new(catColorResult.StatusCode, catColorResult.Message);
            entity.CatColorId = catColorResult.Value!.Id;

            await ApplicationDbContext.SaveChangesAsync(token);

            entity.Color = catColorResult.Value!.ToEFModel();
            return new(StatusCodes.Status200OK, null, ModelToDtoFunc(entity));
        }

        protected override async Task<TEntity?> GetDetailEntityAsync(int id, CancellationToken token = default)
        {
            var result = await ApplicationDbContext.Set<TEntity>()
                .Include(c => c.Color)
                .Include(k => k.Photos)
                .ThenInclude(p => p.TelegramPhoto)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id, token);

            if (result != null)
                SortPhotosByDefault(result.Photos);

            return result;
        }
    }
}
