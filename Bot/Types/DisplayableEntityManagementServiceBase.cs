using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Bot.Interfaces.Services.Management.Base;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public abstract class DisplayableEntityManagementServiceBase<TEntity, TDto>(
        IDisplayableEntityApiClient<TDto> apiClient,
        IMessagesProvider messagesProvider,
        IPhotosDownloaderService photosDownloaderService,
        IEntityFormService entityFormService,
        ILogger logger) : IDisplayableEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity where TDto : class
    {
        private readonly IDisplayableEntityApiClient<TDto> _apiClient = apiClient;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly ILogger _logger = logger;

        protected abstract Func<TDto?, TEntity?> DtoToEntityFunc { get; }

        public abstract Task<ServiceResult<TEntity>> AddNewEntityAsync(CancellationToken token = default);

        public abstract Task<ServiceResult<PagedResult<TEntity>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);

        public virtual async Task<ServiceResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.GetPhotosLimitAsync(photosType, token);
                return result != null
                    ? new(result.StatusCode, result.Message, result.Value)
                    : new(500, _messagesProvider.CreateUnknownErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить лимит фотографий {type}!", photosType);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult> DeleteEntityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.DeleteAsync(entityId, token);
                if (result.Success)
                    return new(result.StatusCode, result.Message);

                return new(500, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить {type} {id}", typeof(TEntity).Name, entityId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult<TEntity>> SetDefaultPhotoAsync(int entityId, int photoId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.SetDefaultPhotoAsync(entityId, photoId, token);

                return result != null 
                    ? new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value)) 
                    : new(500, _messagesProvider.CreateDefaultPhotoSetErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке заглавного фото для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult<TEntity>> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var result = await _apiClient.GetAsync(entityId, token);
            return new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value));
        }

        public virtual async Task<ServiceResult<TEntity>> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default)
        {
            if (photosType != PhotosType.Photos)
            {
                return new(500, _messagesProvider.CreateInvalidPhotoTypeSupportMessage<TEntity>(photosType));
            }
            if (photos == null || photos.Length == 0)
            {
                return new(500, _messagesProvider.CreateNoPhotosToUploadMessage());
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
                return new(500, _messagesProvider.CreatePhotoDownloadFailedMessage());

            try
            {
                var uploadResults = await _apiClient.UploadPhotosAsync(entityId, filesToUpload, token);

                if (uploadResults?.Value?.Results.Any(p => !p.Uploaded) == true)
                    return new(500, _messagesProvider.CreateApiUploadFailedMessage());

                string? warning = anyDownloadFailed ? _messagesProvider.CreatePhotoDownloadFailedMessage() : null;
                return new(uploadResults!.StatusCode, warning, DtoToEntityFunc(uploadResults!.Value!.Dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке фотографий для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
            finally
            {
                foreach (var (fileStream, _, _) in filesToUpload)
                {
                    await fileStream.DisposeAsync();
                }
            }
        }

        public virtual async Task<ServiceResult<TEntity>> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token)
        {

            try
            {
                var result = await _apiClient.DeletePhotosAsync(entityId, photoIds, token);

                if (result != null)
                    return new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value));

                return new(500, _messagesProvider.CreatePhotosDeletionFailureMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления фотографий для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult<TEntity>> SetDefaultPhotoToEntityAsync(int entityId, int photoId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.SetDefaultPhotoAsync(entityId, photoId, token);

                return result != null 
                    ? new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value)) 
                    : new(500, _messagesProvider.CreateDefaultPhotoSetErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке заглавного фото для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult<TEntity>> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.ToggleVisibilityAsync(entityId, token);

                if (result != null)
                    return new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value));

                return new(500, _messagesProvider.CreateUnknownErrorMessage());
            }
            catch (Exception ex)
            {
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ServiceResult<TEntity>> UpdateEntityAsync(
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token = default)
        {
            try
            {
                var dto = new Dictionary<string, string>();
                foreach (var (propertyName, value) in properties)
                {
                    var property = _entityFormService.GetPropertyName<TEntity>(propertyName);
                    if (property != null)
                        dto.Add(property, value);
                }

                var result = await _apiClient.UpdateAsync(modelId, dto, token);
                return new(result.StatusCode, result.Message, DtoToEntityFunc(result.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении CatColor {id} через Reply", modelId);
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
