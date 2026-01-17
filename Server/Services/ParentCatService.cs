using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Records;
using BlueBellDolls.Server.Settings;
using BlueBellDolls.Server.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Services
{
    public class ParentCatService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        IEntityFactory entityFactory,
        ILogger<ParentCatService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : CatServiceBase<ParentCat, ParentCatDetailDto>(env, applicationDbContext, fileStorageSettings, logger), IParentCatService
    {

        #region Fields

        private readonly ILogger<ParentCatService> _logger = logger;
        private readonly IEntityFactory _entityFactory = entityFactory;

        #endregion

        #region CRUD

        public async Task<ServiceResult<PagedResult<ParentCatListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, bool? isMale, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.Cats
                    .AsNoTracking()
                    .Where(c => admin || c.IsEnabled);

                if (isMale.HasValue)
                    query = query.Where(c => c.IsMale == isMale.Value);

                var items = await query
                    .Include(c => c.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .OrderBy(c => c.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(token);

                var totalItems = await query.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var paged = new PagedResult<ParentCatListDto>([.. items.Select(p => p.ToListDto())], pageNumber, pageSize, totalItems, totalPages);
                return new ServiceResult<PagedResult<ParentCatListDto>>(StatusCodes.Status200OK, null, paged);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(ParentCatService), nameof(GetListAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу ParentCat!");
                return new ServiceResult<PagedResult<ParentCatListDto>>(StatusCodes.Status500InternalServerError, "Не удалось получить страницу ParentCat!");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default)
        {
            try
            {
                var result = await GetDetailEntityAsync(id, token);

                if (result == null)
                {
                    _logger.LogWarning("ParentCat {id} не найден.", id);
                    return new ServiceResult<ParentCatDetailDto>(StatusCodes.Status404NotFound, "Производитель не найден");
                }
                if (!admin && result.IsEnabled)
                    return new(StatusCodes.Status403Forbidden, "Доступ к коту запрещён");

                return new(StatusCodes.Status200OK, null, result.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(ParentCatService), nameof(GetAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить ParentCat {id}!", id);
                return new ServiceResult<ParentCatDetailDto>(StatusCodes.Status500InternalServerError, "Не удалось получить производителя");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await GetDetailEntityAsync(id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Производитель не найден");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                foreach (var litter in await ApplicationDbContext.Litters
                    .Where(l => l.MotherCatId == id || l.FatherCatId == id)
                    .ToListAsync(token))
                {
                    if (litter.MotherCatId == id)
                        litter.MotherCatId = null;

                    if (litter.FatherCatId == id)
                        litter.FatherCatId = null;
                }

                ApplicationDbContext.Cats.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new (StatusCodes.Status200OK);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(ParentCatService), nameof(DeleteAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить ParentCat {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось удалить производителя");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> AddAsync(CreateParentCatDto parentCatDto, CancellationToken token = default)
        {
            try
            {
                var entity = _entityFactory.CreateNewParentCat(
                    parentCatDto.Name,
                    parentCatDto.IsMale,
                    parentCatDto.Description,
                    parentCatDto.Color);
                await ApplicationDbContext.Cats.AddAsync(entity, token);
                await ApplicationDbContext.SaveChangesAsync(token);

                return new(StatusCodes.Status201Created, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(ParentCatService), nameof(AddAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить ParentCat!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось добавить производителя");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> UpdateAsync(int id, UpdateParentCatDto parentCatDto, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Cats.FindAsync([id], token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Производитель не найден");

                if (entity.IsMale != parentCatDto.IsMale)
                {
                    var littersToUpdate = await ApplicationDbContext.Litters
                    .Where(
                        l => (l.MotherCatId == id && parentCatDto.IsMale) ||
                        (l.FatherCatId == id && !parentCatDto.IsMale))
                    .ToListAsync(token);

                    foreach (var litter in littersToUpdate)
                    {
                        if (parentCatDto.IsMale) litter.MotherCatId = null;
                        else litter.FatherCatId = null;
                    }
                }

                entity.ApplyUpdate(parentCatDto);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, Value: entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(ParentCatService), nameof(UpdateAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить ParentCat {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обновить производителя");
            }
        }

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override Func<ParentCat, ParentCatDetailDto> ModelToDtoFunc => (model) => model.ToDetailDto();

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath)) return false;
            if (storagePath is not ("photos" or "titles" or "gentests")) return false;

            photosType = storagePath switch
            {
                "photos" => PhotosType.Photos,
                "titles" => PhotosType.Titles,
                "gentests" => PhotosType.GenTests,
                _ => throw new InvalidOperationException(nameof(storagePath))
            };
            return true;
        }

        #endregion

    }
}
