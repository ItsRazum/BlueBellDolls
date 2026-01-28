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
    public class LitterService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        ILogger<LitterService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : DisplayableEntityServiceBase<Litter, LitterDetailDto>(env, applicationDbContext, fileStorageSettings, logger), ILitterService
    {

        #region Fields

        private readonly ILogger<LitterService> _logger = logger;
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;

        #endregion

        #region CRUD

        public async Task<ServiceResult<PagedResult<LitterDetailDto>>> GetDetailListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.Litters
                    .AsNoTracking()
                    .Where(l => admin || l.IsEnabled);

                var totalItems = await query.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalPages > 0 && totalPages < pageNumber)
                    return new(StatusCodes.Status400BadRequest, "Запрошенная страница находится за пределами диапазона доступных!");

                var items = await query
                    .Include(l => l.Kittens.Where(k => admin || k.IsEnabled))
                    .ThenInclude(k => k.Photos)
                    .Include(l => l.Kittens.Where(k => admin || k.IsEnabled))
                    .ThenInclude(k => k.Color)
                    .Include(l => l.MotherCat)
                    .Include(l => l.FatherCat)
                    .Include(l => l.Photos)
                    .ThenInclude(l => l.TelegramPhoto)
                    .AsSplitQuery()
                    .Where(l => l.Kittens.Count != 0)
                    .OrderBy(c => c.Letter)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(token);

                return new(StatusCodes.Status200OK, null, new([.. items.Select(l => l.ToDetailDto(admin))], pageNumber, pageSize, totalItems, totalPages));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(GetDetailListAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Litter!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить страницу Litter");
            }
        }

        public async Task<ServiceResult<PagedResult<LitterMinimalDto>>> GetMinimalListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var query = ApplicationDbContext.Litters
                    .Where(l => admin || l.IsEnabled);

                var totalItems = await query.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (totalPages < pageNumber)
                    return new(StatusCodes.Status400BadRequest, "Запрошенная страница находится за пределами диапазона доступных!");

                var items = await query
                    .OrderBy(c => c.Letter)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(token);

                return new(StatusCodes.Status200OK, null, new([.. items.Select(l => l.ToMinimalDto())], pageNumber, pageSize, totalItems, totalPages));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(GetMinimalListAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Litter!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить страницу Litter");

            }
        }

        public async Task<ServiceResult<LitterDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default)
        {
            try
            {
                var result = await GetDetailEntityAsync(id, token);
                if (result == null)
                    return new ServiceResult<LitterDetailDto>(StatusCodes.Status404NotFound, $"Litter с id={id} не найден");

                if (!admin && !result.IsEnabled)
                    return new ServiceResult<LitterDetailDto>(StatusCodes.Status401Unauthorized, $"Доступ к помёту запрещён!");

                result.Photos = SortPhotosByDefault(result.Photos);

                var dto = result.ToDetailDto(admin);
                dto.Kittens.RemoveAll(k => !admin && !k.IsEnabled);

                return new ServiceResult<LitterDetailDto>(StatusCodes.Status200OK, null, dto);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(GetAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить Litter {id}!", id);
                return new ServiceResult<LitterDetailDto>(StatusCodes.Status500InternalServerError, "Не удалось получить Litter");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Litters.Include(l => l.Photos).FirstOrDefaultAsync(l => l.Id == id, token);
                if (entity == null)
                    return new ServiceResult(StatusCodes.Status404NotFound, $"Litter с id={id} не найден");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                ApplicationDbContext.Litters.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult(StatusCodes.Status200OK, null);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(DeleteAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить Litter {id}!", id);
                return new ServiceResult(StatusCodes.Status500InternalServerError, "Не удалось удалить Litter");
            }

        }

        public async Task<ServiceResult<LitterDetailDto>> AddAsync(CreateLitterDto litterDto, CancellationToken token = default)
        {
            try
            {
                var lastLetter = await ApplicationDbContext.Litters
                    .Select(l => l.Letter)
                    .OrderByDescending(l => l)
                    .FirstOrDefaultAsync(token);
                var nextLetter = lastLetter switch
                {
                    default(char) => 'A',
                    'Z' => 'A',
                    _ => (char)(lastLetter + 1),
                };

                var entity = litterDto.ToEFModel(nextLetter);
                entity.Kittens = [];
                entity.Photos = [];

                await ApplicationDbContext.Litters.AddAsync(entity, token);
                await ApplicationDbContext.SaveChangesAsync(token);

                return new ServiceResult<LitterDetailDto>(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(AddAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить Litter!");
                return new ServiceResult<LitterDetailDto>(StatusCodes.Status500InternalServerError, "Не удалось добавить Litter");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> AddKittenToLitter(int litterId, CreateKittenDto kittenDto, CancellationToken token = default)
        {
            try
            {
                var litter = await ApplicationDbContext.Litters.Include(l => l.Kittens).FirstOrDefaultAsync(l => l.Id == litterId, token);
                if (litter == null)
                    return new ServiceResult<KittenDetailDto>(StatusCodes.Status404NotFound, $"Litter с id={litterId} не найден");

                var kitten = kittenDto.ToEFModel(litter.BirthDay);
                kitten.Photos = [];
                litter.Kittens.Add(kitten);

                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult<KittenDetailDto>(StatusCodes.Status200OK, null, kitten.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(AddKittenToLitter));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить котёнка в Litter!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось добавить котёнка в Litter");
            }

        }

        public async Task<ServiceResult<SetParentCatForLitterResponse>> SetParentCatAsync(int litterId, int parentCatId, CancellationToken token = default)
        {
            try
            {
                var litter = await GetDetailEntityAsync(litterId, token);
                if (litter == null)
                    return new(StatusCodes.Status404NotFound, $"Litter с id={litterId} не найден");

                var parentCat = await ApplicationDbContext.Cats.FindAsync([parentCatId], token);

                if (parentCat == null)
                    return new(StatusCodes.Status404NotFound, $"ParentCat с id={parentCatId} не найден");

                if (parentCat.IsMale)
                    litter.FatherCat = parentCat;
                else
                    litter.MotherCat = parentCat;

                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, Value: new(parentCat.IsMale, litter.ToDetailDto()));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(SetParentCatAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить родителя в Litter!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось установить родителя в Litter");
            }

        }

        public async Task<ServiceResult<LitterDetailDto>> UpdateAsync(int id, UpdateLitterDto litterDto, CancellationToken token = default)
        {
            try
            {
                if (litterDto.Description != null && string.IsNullOrWhiteSpace(litterDto.Description))
                    return new(StatusCodes.Status400BadRequest, "Описание не может быть пустым!");

                var entity = await GetDetailEntityAsync(id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, $"Litter с id={id} не найден");

                entity.ApplyUpdate(litterDto);
                entity.IsEnabled = false;
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, Value: entity.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(LitterService), nameof(UpdateAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Litter {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обновить Litter");
            }
        }

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override Func<Litter, LitterDetailDto> ModelToDtoFunc => (model) => model.ToDetailDto();

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath)) return false;
            if (storagePath != "photos") return false;

            photosType = PhotosType.Photos;
            return true;
        }

        protected override async Task<Litter?> GetDetailEntityAsync(int id, CancellationToken token = default)
        {
            var result = await ApplicationDbContext.Set<Litter>()
                .Include(l => l.Kittens)
                .ThenInclude(k => k.Photos)
                .Include(l => l.Kittens)
                .ThenInclude(k => k.Color)
                .Include(l => l.FatherCat)
                .Include(l => l.MotherCat)
                .Include(l => l.Kittens)
                .Include(l => l.Photos)
                .ThenInclude(p => p.TelegramPhoto)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id == id, token);

            if (result != null)
                SortPhotosByDefault(result.Photos);

            return result;
        }

        #endregion

    }
}
