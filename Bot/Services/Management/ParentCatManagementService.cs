using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Services.Management;
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
            entityFormService,
            logger), IParentCatManagementService
    {
        private readonly IParentCatApiClient _parentCatApiClient = parentCatApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly IPhotosDownloaderService _photosDownloaderService = photosDownloaderService;
        private readonly ILogger<ParentCatManagementService> _logger = logger;

        protected override Func<ParentCatDetailDto?, ParentCat?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

        public override async Task<ServiceResult<ParentCat>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateParentCatDto(
                    Name: string.Empty,
                    BirthDay: DateOnly.FromDateTime(DateTime.Now),
                    IsMale: true,
                    Description: string.Empty);

                var result = await _parentCatApiClient.AddAsync(dto, token);
                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить ParentCat!");
                return new(500, $"Не удалось добавить ParentCat: {ex.Message}");
            }
        }

        public async Task<ServiceResult<PagedResult<ParentCat>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _parentCatApiClient.GetByPageAsync(isMale, pageIndex, pageSize, token);

                var page = result.Value;
                if (page == null)
                    return new(result.StatusCode, result.Message);

                return new(result.StatusCode, result.Message, new PagedResult<ParentCat>(
                    [.. page.Items.Select(dto => dto.ToEFModel())],
                    page.PageNumber,
                    page.PageSize,
                    page.TotalItems,
                    page.TotalPages));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу ParentCat!");
                return new(500, $"Не удалось получить страницу ParentCat: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<PagedResult<ParentCat>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _parentCatApiClient.GetByPageAsync(pageIndex, pageSize, token);

                var page = result.Value;
                if (page == null)
                    return new(result.StatusCode, result.Message);

                return new(result.StatusCode, result.Message, new PagedResult<ParentCat>(
                    [.. page.Items.Select(dto => dto.ToEFModel())],
                    page.PageNumber,
                    page.PageSize,
                    page.TotalItems,
                    page.TotalPages));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу ParentCat!");
                return new(500, $"Не удалось получить страницу ParentCat: {ex.Message}");
            }
        }

        public async Task<ServiceResult<ParentCat>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            try
            {
                var result = await _parentCatApiClient.UpdateColorAsync(entityId, color, token);
                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить цвет для ParentCat {id}!", entityId);
                return new(500, $"Не удалось обновить цвет для ParentCat: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<ParentCat>> AddPhotosToEntityAsync(
                int entityId,
                PhotoAdapter[] photos,
                PhotosType photosType,
                CancellationToken token)
        {
            if (photos == null || photos.Length == 0)
                return new(500, _messagesProvider.CreateNoPhotosToUploadMessage());

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
                return new(500, _messagesProvider.CreatePhotoDownloadFailedMessage());

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
                    return new(500, _messagesProvider.CreateApiUploadFailedMessage());

                if (uploadResults.Value?.Results.Any(p => !p.Uploaded) == true)
                    anyDownloadFailed = true;

                var warning = anyDownloadFailed ? _messagesProvider.CreateApiUploadFailedMessage([.. uploadResults.Value?.Results.Select(p => p.FileIndex) ?? []]) : null;
                return new(uploadResults.StatusCode, warning, uploadResults.Value?.Dto.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке файлов для ParentCat {id}!", entityId);
                return new(500, $"Ошибка при загрузке файлов для ParentCat: {ex.Message}");
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
