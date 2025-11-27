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
using System.Net;

namespace BlueBellDolls.Server.Services
{
    public class LitterService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        ILogger<LitterService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : DisplayableEntityServiceBase<Litter>(env, applicationDbContext, logger), ILitterService
    {

        #region Fields

        private readonly ILogger<LitterService> _logger = logger;
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;

        #endregion

        #region CRUD

        public async Task<ServiceResult<PagedResult<LitterDetailDto>>> GetListAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var items = await ApplicationDbContext.Litters
                    .AsNoTracking()
                    .OrderBy(c => c.Letter)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => l.ToDetailDto())
                    .ToListAsync(token);

                var totalItems = await ApplicationDbContext.Litters.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var paged = new PagedResult<LitterDetailDto>(items, pageNumber, pageSize, totalItems, totalPages);
                return new ServiceResult<PagedResult<LitterDetailDto>>((int)HttpStatusCode.OK, null, paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Litter!");
                return new ServiceResult<PagedResult<LitterDetailDto>>((int)HttpStatusCode.InternalServerError, "Не удалось получить страницу Litter");
            }
        }

        public async Task<ServiceResult<LitterDetailDto>> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                var result = await GetEntity(id, token);
                if (result == null)
                    return new ServiceResult<LitterDetailDto>((int)HttpStatusCode.NotFound, $"Litter с id={id} не найден");

                result.Photos = SortPhotosByDefault(result.Photos);

                return new ServiceResult<LitterDetailDto>((int)HttpStatusCode.OK, null, result.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить Litter {id}!", id);
                return new ServiceResult<LitterDetailDto>((int)HttpStatusCode.InternalServerError, "Не удалось получить Litter");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Litters.Include(l => l.Photos).FirstOrDefaultAsync(l => l.Id == id, token);
                if (entity == null)
                    return new ServiceResult((int)HttpStatusCode.NotFound, $"Litter с id={id} не найден");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                ApplicationDbContext.Litters.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult((int)HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить Litter {id}!", id);
                return new ServiceResult((int)HttpStatusCode.InternalServerError, "Не удалось удалить Litter");
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

                return new ServiceResult<LitterDetailDto>((int)HttpStatusCode.OK, null, entity.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить Litter!");
                return new ServiceResult<LitterDetailDto>((int)HttpStatusCode.InternalServerError, "Не удалось добавить Litter");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> AddKittenToLitter(int litterId, CreateKittenDto kittenDto, CancellationToken token = default)
        {
            var litter = await ApplicationDbContext.Litters.Include(l => l.Kittens).FirstOrDefaultAsync(l => l.Id == litterId, token);
            if (litter == null)
                return new ServiceResult<KittenDetailDto>((int)HttpStatusCode.NotFound, $"Litter с id={litterId} не найден");

            var kitten = kittenDto.ToEFModel(litter.BirthDay);
            kitten.Photos = [];
            litter.Kittens.Add(kitten);

            await ApplicationDbContext.SaveChangesAsync(token);
            return new ServiceResult<KittenDetailDto>((int)HttpStatusCode.OK, null, kitten.ToDetailDto());
        }

        public async Task<ServiceResult> SetFatherCatAsync(int litterId, int parentCatId, CancellationToken token = default)
        {
            var litter = await GetEntity(litterId, token);
            if (litter == null)
                return new ServiceResult((int)HttpStatusCode.NotFound, $"Litter с id={litterId} не найден");

            var parentCat = await ApplicationDbContext.Cats.FindAsync([parentCatId], token);

            if (parentCat == null || !parentCat.IsMale)
                return new ServiceResult((int)HttpStatusCode.BadRequest, "Неверный родительский кот");

            litter.FatherCat = parentCat;
            await ApplicationDbContext.SaveChangesAsync(token);
            return new ServiceResult((int)HttpStatusCode.OK, null);
        }

        public async Task<ServiceResult> SetMotherCatAsync(int litterId, int parentCatId, CancellationToken token = default)
        {
            var litter = await GetEntity(litterId, token);
            if (litter == null)
                return new ServiceResult((int)HttpStatusCode.NotFound, $"Litter с id={litterId} не найден");

            var parentCat = await ApplicationDbContext.Cats.FindAsync([parentCatId], token);

            if (parentCat == null || parentCat.IsMale)
                return new ServiceResult((int)HttpStatusCode.BadRequest, "Неверный родительский кот");

            litter.MotherCat = parentCat;
            await ApplicationDbContext.SaveChangesAsync(token);
            return new ServiceResult((int)HttpStatusCode.OK, null);
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateLitterDto litterDto, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.Litters.FindAsync([id], token);
                if (entity == null)
                    return new ServiceResult((int)HttpStatusCode.NotFound, $"Litter с id={id} не найден");

                entity.ApplyUpdate(litterDto);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new ServiceResult((int)HttpStatusCode.OK, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Litter {id}!", id);
                return new ServiceResult((int)HttpStatusCode.InternalServerError, "Не удалось обновить Litter");
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
                PhotosType.Photos => _fileStorageSettings.MaxPhotosPerLitter,
                _ => 0
            };
        }

        #endregion

        #region Private methods

        private async Task<Litter?> GetEntity(int id, CancellationToken token = default)
        {
            return await ApplicationDbContext.Litters
                .Include(l => l.FatherCat)
                .Include(l => l.MotherCat)
                .Include(l => l.Kittens)
                .Include(l => l.Photos)
                .ThenInclude(p => p.TelegramPhoto)
                .AsSplitQuery()
                .FirstOrDefaultAsync(l => l.Id == id, token);
        }

        #endregion

    }
}
