using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Settings;
using BlueBellDolls.Server.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Services
{
    public class KittenService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        ICatColorService catColorService,
        ILogger<KittenService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : CatServiceBase<Kitten, KittenDetailDto>(env, applicationDbContext, fileStorageSettings, catColorService, logger), IKittenService
    {

        #region Fields

        private readonly ILogger<KittenService> _logger = logger;

        #endregion

        #region CRUD

        public async Task<ServiceResult<PagedResult<KittenListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.Kittens
                    .AsNoTracking()
                    .Where(k => admin || k.IsEnabled);

                var totalItems = await query.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalPages > 0 && totalPages < pageNumber)
                    return new(StatusCodes.Status400BadRequest, "Запрошенная страница находится за пределами диапазона доступных!");

                var items = await query
                    .Include(p => p.Photos)
                    .OrderBy(c => c.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(token);
                
                var result = new PagedResult<KittenListDto>([.. items.Select(k => k.ToListDto(admin))], pageNumber, pageSize, totalItems, totalPages);
                return new(StatusCodes.Status200OK, null, result);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(GetListAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Kitten!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить список котят");
            }
        }

        public async Task<ServiceResult<KittenListDto[]>> GetAvailableKittensAsync(CancellationToken token = default)
        {
            try
            {
                var items = await ApplicationDbContext.Kittens
                    .AsNoTracking()
                    .Include(k => k.Color)
                    .Include(k => k.Litter)
                    .Include(p => p.Photos)
                    .Where(k => k.IsEnabled && k.Status == KittenStatus.Available)
                    .Take(25)
                    .ToListAsync(token);

                return new(StatusCodes.Status200OK, null, [.. items.Select(k => k.ToListDto(false))]);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(GetAvailableKittensAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить список доступных котят!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить список доступных котят");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default)
        {
            try
            {
                var result = await GetDetailEntityAsync(id, token);

                if (result == null)
                    return new ServiceResult<KittenDetailDto>(StatusCodes.Status404NotFound, "Котёнок не найден");

                if (!admin && !result.IsEnabled)
                    return new ServiceResult<KittenDetailDto>(StatusCodes.Status401Unauthorized, "Доступ к котёнку запрещён!");

                return new ServiceResult<KittenDetailDto>(StatusCodes.Status200OK, null, result.ToDetailDto(admin));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(GetAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
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
                var entity = await ApplicationDbContext.Kittens
                    .Include(k => k.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .FirstOrDefaultAsync(k => k.Id == id, token);

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
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(DeleteAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить Kitten {id}!", id);
                return new ServiceResult(StatusCodes.Status500InternalServerError, "Не удалось удалить котёнка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateAsync(int id, UpdateKittenDto kittenDto, CancellationToken token = default)
        {
            try
            {
                if (kittenDto.Description != null && string.IsNullOrWhiteSpace(kittenDto.Description))
                    return new(StatusCodes.Status400BadRequest, "Описание не может быть пустым!");

                var entity = await GetDetailEntityAsync(id, token);

                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Котёнок не найден");

                entity.ApplyUpdate(kittenDto);
                entity.IsEnabled = false;
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, Value: entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(UpdateAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Kitten {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обновить котёнка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateKittenClassAsync(int id, UpdateKittenClassRequest request, CancellationToken token = default)
        {
            try
            {
                var entity = await GetDetailEntityAsync(id, token);

                if (entity == null)
                    return new(StatusCodes.Status404NotFound);

                entity.Class = request.Class;
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(UpdateKittenClassAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить класс Kitten {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось установить класс для котёнка");
            }

        }

        public async Task<ServiceResult<KittenDetailDto>> UpdateKittenStatusAsync(int id, UpdateKittenStatusRequest request, CancellationToken token = default)
        {
            try
            {
                var entity = await GetDetailEntityAsync(id, token);

                if (entity == null)
                    return new(StatusCodes.Status404NotFound);

                entity.Status = request.Status;
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(KittenService), nameof(UpdateKittenStatusAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить статус Kitten {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось установить статус для котёнка");

            }

        }

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override Func<Kitten, KittenDetailDto> ModelToDtoFunc => (model) => model.ToDetailDto();

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath)) return false;
            if (storagePath != "photos") return false;

            photosType = PhotosType.Photos;
            return true;
        }

        protected override async Task<Kitten?> GetDetailEntityAsync(int id, CancellationToken token = default)
        {
            var result = await ApplicationDbContext.Set<Kitten>()
                .Include(k => k.Color)
                .Include(k => k.Litter)
                .Include(k => k.Photos)
                .ThenInclude(p => p.TelegramPhoto)
                .AsSplitQuery()
                .FirstOrDefaultAsync(k => k.Id == id, token);

            if (result != null)
                SortPhotosByDefault(result.Photos);

            return result;
        }

        #endregion

    }
}
