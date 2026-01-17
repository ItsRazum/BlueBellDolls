using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public abstract class DisplayableEntityManagementServiceBase<TEntity, TDto>(
        IDisplayableEntityApiClient<TDto> apiClient,
        IMessagesProvider messagesProvider,
        IPhotosDownloaderService photosDownloaderService,
        ILogger logger) : IDisplayableEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity where TDto : class
    {
        private readonly IDisplayableEntityApiClient<TDto> _apiClient = apiClient;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly ILogger _logger = logger;

        protected abstract Func<TDto?, TEntity?> DtoToEntityFunc { get; }

        public abstract Task<ManagementOperationResult<TEntity>> AddNewEntityAsync(CancellationToken token = default);

        public abstract Task<ManagementOperationResult<TEntity>> UpdateEntityAsync(TEntity entity, CancellationToken token = default);

        public abstract Task<ManagementOperationResult<PagedResult<TEntity>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);

        public abstract Task<ManagementOperationResult<TEntity>> UpdateEntityByReplyAsync(int modelId, Dictionary<string, string> properties, CancellationToken token = default);

        public virtual async Task<ManagementOperationResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.GetPhotosLimitAsync(photosType, token);
                return result != null
                    ? new(true, null, result)
                    : new(false, _messagesProvider.CreateUnknownErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить лимит фотографий {type}!", photosType);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var success = await _apiClient.DeleteAsync(entityId, token);
                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить {type} {id}", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult<TEntity>> SetDefaultPhotoAsync(int entityId, int photoId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.SetDefaultPhotoAsync(entityId, photoId, token);

                return result != null 
                    ? new(true, null, DtoToEntityFunc(result)) 
                    : new(false, _messagesProvider.CreateDefaultPhotoSetErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке заглавного фото для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<TEntity?> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var entity = await _apiClient.GetAsync(entityId, token);
            return DtoToEntityFunc(entity);
        }

        public virtual async Task<ManagementOperationResult<TEntity>> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default)
        {
            if (photosType != PhotosType.Photos)
            {
                return new(false, _messagesProvider.CreateInvalidPhotoTypeSupportMessage<TEntity>(photosType));
            }
            if (photos == null || photos.Length == 0)
            {
                return new(false, _messagesProvider.CreateNoPhotosToUploadMessage());
            }

            var filesToUpload = new List<(Stream fileStream, string fileName, string fileId)>();
            bool anyDownloadFailed = false;

            foreach (var photoAdapter in photos)
            {
                var downloadResult = await _photosDownloaderService.DownloadPhotoAsync(photoAdapter, token);
                if (downloadResult != null)
                {
                    filesToUpload.Add(downloadResult.Value);
                }
                else
                {
                    _logger.LogWarning("Не удалось скачать файл {fileId} для {type} {entityId}!", photoAdapter.FileId, typeof(TEntity).Name, entityId);
                    anyDownloadFailed = true;
                }
            }

            if (filesToUpload.Count == 0)
                return new(false, _messagesProvider.CreatePhotoDownloadFailedMessage());

            try
            {
                var uploadResults = await _apiClient.UploadPhotosAsync(entityId, filesToUpload, token);

                if (uploadResults?.Results.Any(p => !p.Uploaded) == true)
                    return new(false, _messagesProvider.CreateApiUploadFailedMessage());

                string? warning = anyDownloadFailed ? _messagesProvider.CreatePhotoDownloadFailedMessage() : null;
                return new(true, warning, DtoToEntityFunc(uploadResults!.Dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке фотографий для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
            finally
            {
                foreach (var (fileStream, _, _) in filesToUpload)
                {
                    await fileStream.DisposeAsync();
                }
            }
        }

        public virtual async Task<ManagementOperationResult<TEntity>> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token)
        {

            try
            {
                var result = await _apiClient.DeletePhotosAsync(entityId, photoIds, token);

                if (result != null)
                    return new(true, null, DtoToEntityFunc(result));

                return new(false, _messagesProvider.CreatePhotosDeletionFailureMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления фотографий для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult<TEntity>> SetDefaultPhotoToEntityAsync(int entityId, int photoId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.SetDefaultPhotoAsync(entityId, photoId, token);

                return result != null 
                    ? new(true, null, DtoToEntityFunc(result)) 
                    : new(false, _messagesProvider.CreateDefaultPhotoSetErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке заглавного фото для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult<TEntity>> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.ToggleVisibilityAsync(entityId, token);

                if (result != null)
                    return new(true, null, DtoToEntityFunc(result));

                return new(false, _messagesProvider.CreateUnknownErrorMessage());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
