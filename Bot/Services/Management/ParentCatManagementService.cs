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
        ILogger<ParentCatManagementService> logger) : DisplayableEntityManagementServiceBase<ParentCat, ParentCatDetailDto>(
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

        protected override Func<ParentCatDetailDto?, ParentCat?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

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
                var result = await _parentCatApiClient.UpdateAsync(entity.Id, updateDto, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(true, null, result.ToEFModel());
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
                        [.. result.Items.Select(dto => dto.ToEFModel())],
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
                        [.. result.Items.Select(dto => dto.ToEFModel())],
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
                var result = await _parentCatApiClient.UpdateColorAsync(entityId, color, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateColorUpdateErrorMessage());

                return new(true, null, result.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<ParentCat>> AddPhotosToEntityAsync(
                int entityId,
                PhotoAdapter[] photos,
                PhotosType photosType,
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
                var uploadResults = photosType switch
                {
                    PhotosType.Photos => await _parentCatApiClient.UploadPhotosAsync(entityId, filesToUpload, token),
                    PhotosType.Titles => await _parentCatApiClient.UploadTitlesAsync(entityId, filesToUpload, token),
                    PhotosType.GenTests => await _parentCatApiClient.UploadGenTestsAsync(entityId, filesToUpload, token),
                    _ => throw new ArgumentOutOfRangeException(nameof(photosType))
                };

                if (uploadResults == null)
                    return new(false, _messagesProvider.CreateApiUploadFailedMessage());

                if (uploadResults.Results.Any(p => !p.Uploaded))
                    anyDownloadFailed = true;

                var warning = anyDownloadFailed ? _messagesProvider.CreateApiUploadFailedMessage([.. uploadResults.Results.Select(p => p.FileIndex)]) : null;
                return new(true, warning, uploadResults.Dto.ToEFModel());
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
    }
}
