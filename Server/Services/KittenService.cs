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
    public class KittenService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        ILogger<KittenService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : CatServiceBase<Kitten>(env, applicationDbContext, logger), IKittenService
    {

        #region Fields

        private readonly ILogger<KittenService> _logger = logger;
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;

        #endregion

        #region CRUD

        public async Task<ServiceResult<PagedResult<KittenListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.Kittens.AsQueryable();
                if (!admin)
                    query = query.Where(k => k.IsEnabled);

                var items = await query
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(k => k.ToListDto())
                    .ToListAsync(token);

                var totalItems = await ApplicationDbContext.Kittens.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var result = new PagedResult<KittenListDto>(items, pageNumber, pageSize, totalItems, totalPages);
                return new ServiceResult<PagedResult<KittenListDto>>(StatusCodes.Status200OK, null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Kitten!");
                return new ServiceResult<PagedResult<KittenListDto>>(StatusCodes.Status500InternalServerError, "Не удалось получить список котят");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default)
        {
            try
            {
                var result = await ApplicationDbContext.Kittens
                    .AsNoTracking()
                    .Include(k => k.Litter)
                    .Include(k => k.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .FirstOrDefaultAsync(k => k.Id == id, token);

                if (result == null)
                    return new ServiceResult<KittenDetailDto>(StatusCodes.Status404NotFound, "Котёнок не найден");

                if (!admin && !result.IsEnabled)
                    return new ServiceResult<KittenDetailDto>(StatusCodes.Status401Unauthorized, "Доступ к котёнку запрещён!");

                result.Photos = SortPhotosByDefault(result.Photos);

                return new ServiceResult<KittenDetailDto>(StatusCodes.Status200OK, null, result.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить Kitten {id}!", id);
                return new ServiceResult<KittenDetailDto>(StatusCodes.Status500InternalServerError, "Не удалось получить данные котёнка");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Kittens.Include(k => k.Photos).FirstOrDefaultAsync(k => k.Id == id, token);
                if (entity == null)
                    return new ServiceResult(StatusCodes.Status404NotFound, "Котёнок не найден");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                ApplicationDbContext.Kittens.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить Kitten {id}!", id);
                return new ServiceResult(StatusCodes.Status500InternalServerError, "Не удалось удалить котёнка");
            }
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateKittenDto kittenDto, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Kittens.FindAsync([id], token);
                if (entity == null)
                    return new ServiceResult(StatusCodes.Status404NotFound, "Котёнок не найден");

                entity.ApplyUpdate(kittenDto);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Kitten {id}!", id);
                return new ServiceResult(StatusCodes.Status500InternalServerError, "Не удалось обновить котёнка");
            }
        }

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath)) return false;
            if (storagePath != "photos") return false;

            photosType = PhotosType.Photos;
            return true;
        }

        protected override int GetPhotosLimit(PhotosType photosType)
        {
            return photosType switch
            {
                PhotosType.Photos => _fileStorageSettings.MaxPhotosPerKitten,
                _ => 0
            };
        }

        #endregion

    }
}
