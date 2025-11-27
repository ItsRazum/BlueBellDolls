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
    public class CatColorService(
        IApplicationDbContext applicationDbContext,
        IWebHostEnvironment env,
        ILogger<CatColorService> logger,
        IOptions<FileStorageSettings> fileStorageSettings) 
        : DisplayableEntityServiceBase<CatColor>(env, applicationDbContext, logger), ICatColorService
    {
        private readonly ILogger<CatColorService> _logger;
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;

        #region Methods

        #region CRUD

        public async Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token)
        {
            try
            {
                var entity = dto.ToEFModel();
                ApplicationDbContext.CatColors.Add(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status201Created, null, entity.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить новый CatColor!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось добавить новый CatColor!");
            }
        }

        public async Task<ServiceResult<PagedResult<CatColorListDto>>> GetListAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var items = await ApplicationDbContext.CatColors
                    .OrderBy(c => c.Identifier)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(k => k.ToListDto())
                    .ToListAsync(token);

                var totalItems = await ApplicationDbContext.CatColors.CountAsync(token);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                var result = new PagedResult<CatColorListDto>(items, pageNumber, pageSize, totalItems, totalPages);
                return new(StatusCodes.Status200OK, null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу CatColor!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить страницу CatColor!");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                var result = await ApplicationDbContext.CatColors
                    .Include(k => k.Photos)
                    .ThenInclude(p => p.TelegramPhoto)
                    .FirstOrDefaultAsync(k => k.Id == id, token);

                if (result == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                result.Photos = SortPhotosByDefault(result.Photos);

                return new(StatusCodes.Status200OK, null, result.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось получить CatColor!");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.CatColors.Include(k => k.Photos).FirstOrDefaultAsync(k => k.Id == id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                foreach (var photo in entity.Photos)
                {
                    TryDeleteFileFromDisk(photo.Url);
                }

                ApplicationDbContext.CatColors.Remove(entity);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось удалить CatColor!");
            }
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateCatColorDto catColorDto, CancellationToken token = default)
        {
            try
            {
                var entity = await ApplicationDbContext.CatColors.FindAsync([id], token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "CatColor не найден!");

                entity.ApplyUpdate(catColorDto);
                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить CatColor {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обновить CatColor!");
            }
        }

        #endregion

        #endregion

        #region DisplayableEntityServiceBase overrides

        protected override bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType)
        {
            photosType = PhotosType.None;
            if (string.IsNullOrEmpty(storagePath) || storagePath != "photos")
                return false;

            photosType = PhotosType.Photos;
            return true;
        }

        protected override int GetPhotosLimit(PhotosType photosType)
        {
            return photosType == PhotosType.Photos ? _fileStorageSettings.MaxPhotosPerCatColor : 0;
        }

        #endregion

    }
}
