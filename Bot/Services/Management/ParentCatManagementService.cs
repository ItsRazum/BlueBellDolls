using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Management
{

    public class ParentCatManagementService(
        IParentCatApiClient parentCatApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IMessagesProvider messagesProvider,
        ILogger<ParentCatManagementService> logger) : DisplayableEntityManagementServiceBase<ParentCat>(
            parentCatApiClient,
            messagesProvider,
            photosDownloaderService,
            logger), IParentCatManagementService
    {
        private readonly IParentCatApiClient _parentCatApiClient = parentCatApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly ILogger<ParentCatManagementService> _logger = logger;

        public override async Task<ParentCat?> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var dto = await _parentCatApiClient.GetAsync(entityId, token);
            return dto?.ToEFModel();
        }

        public override async Task<ManagementOperationResult<ParentCat>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateParentCatDto(
                    Name: "Новый производитель",
                    BirthDay: DateOnly.FromDateTime(DateTime.Now),
                    IsMale: true,
                    Description: "Добавьте описание!",
                    Color: "Не указан");

                var result = await _parentCatApiClient.AddAsync(dto, token);
                if (result != null)
                    return new(true, null, result.ToEFModel());

                return new(false, _messagesProvider.CreateEntityAdditionErrorMessage());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<ParentCat>> UpdateEntityByReplyAsync(
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token = default)
        {
            try
            {
                var currentDto = await _parentCatApiClient.GetAsync(modelId, token);
                if (currentDto == null)
                    return new(false, _messagesProvider.CreateApiGetEntityFailureMessage());

                var tempEfModel = currentDto.ToEFModel();

                foreach (var (propertyName, value) in properties)
                {
                    if (!_entityFormService.UpdateProperty(tempEfModel, propertyName, value))
                        return new(false, _messagesProvider.CreatePropertyUpdateFailureMessage(propertyName));
                }

                return await UpdateEntityAsync(tempEfModel, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в UpdateEntityByReplyAsync для ParentCat {id}", modelId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<ParentCat>> UpdateEntityAsync(ParentCat entity, CancellationToken token = default)
        {
            try
            {
                var updateDto = entity.ToUpdateDto();
                var success = await _parentCatApiClient.UpdateAsync(entity.Id, updateDto, token);
                if (!success)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                var finalDto = await _parentCatApiClient.GetAsync(entity.Id, token);
                if (finalDto == null)
                    return new(false, _messagesProvider.CreateApiGetEntityAfterUpdateFailureMessage());

                return new(true, null, finalDto.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Kitten {id}!", entity.Id);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<PagedResult<ParentCat>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _parentCatApiClient.GetByPageAsync(isMale, pageIndex, pageSize, token);

                if (result != null)
                {
                    return new(true, null, new PagedResult<ParentCat>(
                        [.. result.Items.Select(dto => dto.ToEFModel())], // Ручной маппинг
                        result.PageNumber,
                        result.PageSize,
                        result.TotalItems,
                        result.TotalPages));
                }

                return new(false, _messagesProvider.CreateApiGetPageFailureMessage<ParentCat>());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<PagedResult<ParentCat>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _parentCatApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    return new(true, null, new PagedResult<ParentCat>(
                        [.. result.Items.Select(dto => dto.ToEFModel())], // Ручной маппинг
                        result.PageNumber,
                        result.PageSize,
                        result.TotalItems,
                        result.TotalPages));
                }

                return new(false, _messagesProvider.CreateApiGetPageFailureMessage<ParentCat>());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<ParentCat>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            try
            {
                var success = await _parentCatApiClient.UpdateColorAsync(entityId, color, token);
                if (!success)
                    return new(false, _messagesProvider.CreateColorUpdateErrorMessage());

                var finalDto = await _parentCatApiClient.GetAsync(entityId, token);
                if (finalDto == null)
                    return new(false, _messagesProvider.CreateApiGetEntityAfterUpdateFailureMessage());

                return new(true, null, finalDto.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult> AddPhotosToEntityAsync(
                int entityId,
                PhotoAdapter[] photos,
                PhotosType photosManagementMode,
                CancellationToken token)
        {
            if (photos == null || photos.Length == 0)
                return new(false, _messagesProvider.CreateNoPhotosToUploadMessage());

            var filesToUpload = new List<(Stream fileStream, string fileName, string fileId)>();
            var anyDownloadFailed = false;

            foreach (var photoAdapter in photos)
            {
                var downloadResult = await _photosDownloaderService.DownloadPhotoAsync(photoAdapter, token);
                if (downloadResult != null)
                {
                    filesToUpload.Add(downloadResult.Value);
                }
                else
                {
                    _logger.LogWarning("Не удалось скачать файл {FileId} для ParentCat {EntityId}", photoAdapter.FileId, entityId);
                    anyDownloadFailed = true;
                }
            }

            if (filesToUpload.Count == 0)
                return new(false, _messagesProvider.CreatePhotoDownloadFailedMessage());

            try
            {
                var uploadResults = photosManagementMode switch
                {
                    PhotosType.Photos => await _parentCatApiClient.UploadPhotosAsync(entityId, filesToUpload, token),
                    PhotosType.Titles => await _parentCatApiClient.UploadTitlesAsync(entityId, filesToUpload, token),
                    PhotosType.GenTests => await _parentCatApiClient.UploadGenTestsAsync(entityId, filesToUpload, token),
                    _ => throw new ArgumentOutOfRangeException(nameof(photosManagementMode))
                };

                if (uploadResults == null)
                    return new(false, _messagesProvider.CreateApiUploadFailedMessage());

                if (uploadResults.Any(p => !p.Uploaded))
                    anyDownloadFailed = true;

                var warning = anyDownloadFailed ? _messagesProvider.CreateApiUploadFailedMessage([.. uploadResults.Select(p => p.FileIndex)]) : null;
                return new(true, warning);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файлов для ParentCat {id}!", entityId);
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

        public override async Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token)
        {
            try
            {
                var success = await _parentCatApiClient.DeleteAsync(entityId, token);

                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult> DeleteEntityPhotosAsync(
            int entityId,
            int[] photoIds,
            CancellationToken token = default)
        {
            try
            {
                var success = await _parentCatApiClient.DeletePhotosAsync(entityId, photoIds, token);

                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreatePhotosDeletionFailureMessage());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
