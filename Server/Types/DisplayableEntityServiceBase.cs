using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Records;
using BlueBellDolls.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Types
{
    public abstract class DisplayableEntityServiceBase<TEntity, TDto>(
        IWebHostEnvironment env,
        IApplicationDbContext applicationDbContext,
        IOptions<FileStorageSettings> fileStorageSettings,
        ILogger logger) where TEntity : class, IDisplayableEntity where TDto : class
    {

        #region Fields

        private readonly IWebHostEnvironment _env = env;
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;
        private readonly ILogger _logger = logger;

        #endregion

        #region Properties

        protected IApplicationDbContext ApplicationDbContext { get; } = applicationDbContext;

        protected abstract Func<TEntity, TDto> ModelToDtoFunc { get; }

        #endregion

        #region Methods

        #region Abstract methods

        protected abstract bool ValidatePhotosStoragePath(string storagePath, out PhotosType photosType);

        protected virtual int GetPhotosLimitValue(PhotosType photosType)
        {
            return _fileStorageSettings.Limits[PhotosType.Photos][typeof(TEntity).Name];
        }

        protected virtual async Task<TEntity?> GetDetailEntityAsync(int id, CancellationToken token = default)
        {
            var result = await ApplicationDbContext.Set<TEntity>()
                .Include(k => k.Photos)
                .ThenInclude(p => p.TelegramPhoto)
                .FirstOrDefaultAsync(p => p.Id == id, token);

            if (result != null)
                SortPhotosByDefault(result.Photos);

            return result;
        }

        #endregion

        #region Public methods

        public virtual ServiceResult<PhotosLimitResponse> GetPhotosLimit(PhotosType photosType)
        {
            return new(StatusCodes.Status200OK, null, new PhotosLimitResponse(photosType, GetPhotosLimitValue(photosType)));
        }

        public virtual async Task<ServiceResult<TDto>> ToggleVisibilityAsync(int id, CancellationToken token = default)
        {
            try
            {
                var entity = await GetDetailEntityAsync(id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Сущность не найдена");

                entity.IsEnabled = !entity.IsEnabled;
                await ApplicationDbContext.SaveChangesAsync(token);

                return new(StatusCodes.Status200OK, null, ModelToDtoFunc(entity));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{class}.{method}(): Операция была отменена", nameof(DisplayableEntityServiceBase<TEntity, TDto>), nameof(ToggleVisibilityAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось переключить видимость сущности {id}!", id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось переключить видимость");
            }
        }

        public virtual async Task<ServiceResult<EntityFilesUploadResult<TDto>>> UploadFilesAsync(
            int id,
            string dictionaryName,
            IEnumerable<IFormFile> files,
            List<string>? telegramFileIds = null,
            CancellationToken token = default)
        {
            try
            {
                if (!ValidatePhotosStoragePath(dictionaryName, out var photosType))
                {
                    _logger.LogError("Неверный тип фотографий для {type} {id}! ({dictionaryName})", typeof(TEntity).Name, id, dictionaryName);
                    return new(StatusCodes.Status403Forbidden, "Неверный тип фотографий");
                }

                var entity = await GetDetailEntityAsync(id, token);

                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Сущность не найдена");

                if (files.Count() + entity.Photos.Count(p => p.Type == photosType) >= GetPhotosLimitValue(photosType))
                {
                    _logger.LogError("Превышен лимит загрузки фотографий для {type}! ({photosType})", typeof(TEntity).Name, photosType);
                    return new(StatusCodes.Status403Forbidden, "Превышен лимит загрузки фотографий");
                }

                var addedPhotosResult = new List<(int photoIndex, EntityPhoto? photo)>();
                var newDbPhotos = new List<EntityPhoto>();
                var newTelegramPhotos = new List<TelegramPhoto>();

                var filesList = files.ToList();

                bool useTelegramIds = telegramFileIds != null && telegramFileIds.Count > 0;
                if (useTelegramIds && filesList.Count != telegramFileIds?.Count)
                {
                    _logger.LogError("Ошибка сшивки: {count1} файлов, но {count2} FileId.", filesList.Count, telegramFileIds!.Count);
                    return new(StatusCodes.Status500InternalServerError, "Ошибка сшивки файлов и FileId", new(ModelToDtoFunc(entity),
                        [.. Enumerable.Range(1, filesList.Count).Select(i => new FileUploadResult(i, false))]));
                }

                for (int i = 0; i < filesList.Count; i++)
                {
                    var file = filesList[i];
                    var photoIndex = i + 1;

                    if (file.Length == 0)
                    {
                        addedPhotosResult.Add((photoIndex, null));
                        continue;
                    }

                    try
                    {
                        var relativeDir = Path.Combine("images", $"{typeof(TEntity).Name.ToLower()}s", id.ToString(), dictionaryName);
                        var absoluteDir = Path.Combine(_env.WebRootPath, relativeDir);
                        Directory.CreateDirectory(absoluteDir);

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var absolutePath = Path.Combine(absoluteDir, fileName);

                        using (var stream = new FileStream(absolutePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream, token);
                        }
                        var fileUrl = Path.Combine("/", relativeDir, fileName).Replace(Path.DirectorySeparatorChar, '/');

                        var photo = new EntityPhoto
                        {
                            Url = fileUrl,
                            Type = photosType,
                            IsMain = false
                        };

                        newDbPhotos.Add(photo);
                        addedPhotosResult.Add((photoIndex, photo));

                        if (useTelegramIds)
                        {
                            newTelegramPhotos.Add(new TelegramPhoto
                            {
                                EntityPhoto = photo,
                                FileId = telegramFileIds![i]
                            });
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Загрузка файла отменена для {type} {id}.", typeof(TEntity).Name, id);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Не удалось загрузить файл для {type} {id}!", typeof(TEntity).Name, id);
                        addedPhotosResult.Add((photoIndex, null));
                    }
                }

                if (newDbPhotos.Count != 0)
                {
                    await using var transaction = await ApplicationDbContext.Database.BeginTransactionAsync(token);
                    try
                    {
                        entity.Photos.AddRange(newDbPhotos);
                        await ApplicationDbContext.SaveChangesAsync(token);

                        if (newTelegramPhotos.Count != 0)
                        {
                            await ApplicationDbContext.TelegramPhotos.AddRangeAsync(newTelegramPhotos, token);
                            await ApplicationDbContext.SaveChangesAsync(token);
                        }

                        await transaction.CommitAsync(token);
                    }
                    catch (TaskCanceledException)
                    {
                        _logger.LogWarning("{class}.{method}(): Операция была отменена", nameof(DisplayableEntityServiceBase<TEntity, TDto>), nameof(UploadFilesAsync));
                        await transaction.RollbackAsync(token);
                        return new(StatusCodes.Status403Forbidden, "Операция отменена", new(ModelToDtoFunc(entity),
                            [.. addedPhotosResult.Select(p => new FileUploadResult(p.photoIndex, false))]));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Транзакция загрузки фото не удалась, производится откат!");
                        await transaction.RollbackAsync(token);
                        return new(StatusCodes.Status500InternalServerError, "Ошибка при сохранении фото в базу данных", new(ModelToDtoFunc(entity),
                            [.. addedPhotosResult.Select(p => new FileUploadResult(p.photoIndex, false))]));
                    }
                }

                return new(StatusCodes.Status200OK, null, new(ModelToDtoFunc(entity),
                    [.. addedPhotosResult.Select(p => new FileUploadResult(p.photoIndex, p.photo != null))]));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{class}.{method}(): Операция была отменена", nameof(DisplayableEntityServiceBase<TEntity, TDto>), nameof(UploadFilesAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось загрузить файлы для {type} {id}!", typeof(TEntity).Name, id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось загрузить файлы");
            }
        }

        public virtual async Task<ServiceResult<TDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token)
        {
            try
            {
                var entity = await ApplicationDbContext.Set<TEntity>().Include(e => e.Photos).FirstOrDefaultAsync(e => e.Id == id, token);

                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Сущность не найдена");

                var photo = entity.Photos.FirstOrDefault(p => p.Id == photoId);
                if (photo == null)
                    return new(StatusCodes.Status404NotFound, "Фотография не найдена");

                foreach (var p in entity.Photos)
                {
                    p.IsMain = p.Id == photoId;
                }

                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, ModelToDtoFunc(entity));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{class}.{method}(): Операция была отменена", nameof(DisplayableEntityServiceBase<TEntity, TDto>), nameof(SetDefaultPhotoAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить основную фотографию для {type} {id}!", typeof(TEntity).Name, id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось установить основную фотографию");
            }
        }

        public virtual async Task<ServiceResult<TDto>> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token)
        {
            try
            {
                var entity = await ApplicationDbContext.Set<TEntity>().Include(p => p.Photos).FirstOrDefaultAsync(e => e.Id == id, token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Сущность не найдена");

                foreach(var photoId in photoIds)
                {
                    var photo = entity.Photos.FirstOrDefault(p => p.Id == photoId);
                    if (photo == null) continue;
                    
                    ApplicationDbContext.Photos.Remove(photo);
                    TryDeleteFileFromDisk(photo.Url);
                }

                await ApplicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, ModelToDtoFunc(entity));
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{class}.{method}(): Операция была отменена", nameof(DisplayableEntityServiceBase<TEntity, TDto>), nameof(DeleteFilesAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить файлы для {type} {id}!", typeof(TEntity).Name, id);
                return new(StatusCodes.Status500InternalServerError, "Не удалось удалить файлы");
            }
        }

        #endregion

        #region Protected methods

        protected void TryDeleteFileFromDisk(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;
            try
            {
                var relativePath = fileUrl.TrimStart('/');
                var absolutePath = Path.Combine(_env.WebRootPath, relativePath);
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении файла '{fileUrl}' с диска.", fileUrl);
            }
        }

        protected List<EntityPhoto> SortPhotosByDefault(IEnumerable<EntityPhoto> photos)
        {
            return [.. photos
                .OrderByDescending(p => p.IsMain)
                .ThenBy(p => p.Type)
                .ThenBy(p => p.Id)];
        }

        #endregion

        #endregion

    }
}
