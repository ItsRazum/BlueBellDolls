using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Bot.Interfaces.Management.Base;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Types
{
    public abstract class DisplayableEntityManagementServiceBase<TEntity>(
        IDisplayableEntityApiClient apiClient,
        IMessagesProvider messagesProvider,
        IPhotosDownloaderService photosDownloaderService,
        ILogger logger) : IDisplayableEntityManagementService<TEntity> where TEntity : class, IDisplayableEntity
    {
        private readonly IDisplayableEntityApiClient _apiClient = apiClient;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly ILogger _logger = logger;

        public abstract Task<ManagementOperationResult<TEntity>> AddNewEntityAsync(CancellationToken token = default);

        public abstract Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token = default);

        public abstract Task<ManagementOperationResult<TEntity>> UpdateEntityAsync(TEntity entity, CancellationToken token = default);

        public abstract Task<ManagementOperationResult<PagedResult<TEntity>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);

        public abstract Task<TEntity?> GetEntityAsync(int entityId, CancellationToken token = default);

        public abstract Task<ManagementOperationResult<TEntity>> UpdateEntityByReplyAsync(int modelId, Dictionary<string, string> properties, CancellationToken token = default);


        public virtual async Task<ManagementOperationResult> AddPhotosToEntityAsync(int entityId, PhotoAdapter[] photos, PhotosType photosType, CancellationToken token = default)
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

                if (uploadResults?.Any(p => !p.Uploaded) == true)
                    return new(false, _messagesProvider.CreateApiUploadFailedMessage());

                string? warning = anyDownloadFailed ? _messagesProvider.CreatePhotoDownloadFailedMessage() : null;
                return new(true, warning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке фотографий для {Type} {id}!", typeof(TEntity).Name, entityId);
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

        public virtual async Task<ManagementOperationResult> DeleteEntityPhotosAsync(int entityId, int[] photoIds, CancellationToken token)
        {

            try
            {
                var success = await _apiClient.DeletePhotosAsync(entityId, photoIds, token);

                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreatePhotosDeletionFailureMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка удаления фотографий для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult> SetDefaultPhotoAsync(int entityId, int photoId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.SetDefaultPhotoAsync(entityId, photoId, token);

                return result ? new(true) : new(false, _messagesProvider.CreateDefaultPhotoSetErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при установке заглавного фото для {type} {id}!", typeof(TEntity).Name, entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public virtual async Task<ManagementOperationResult> ToggleEntityVisibilityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var result = await _apiClient.ToggleVisibilityAsync(entityId, token);

                if (result)
                    return new(true);

                return new(false, _messagesProvider.CreateUnknownErrorMessage());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
