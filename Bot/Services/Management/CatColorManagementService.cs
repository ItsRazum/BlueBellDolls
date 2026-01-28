using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Management
{
    public class CatColorManagementService(
        ICatColorApiClient catColorApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IMessagesProvider messagesProvider,
        ILogger<CatColorManagementService> logger)
        : DisplayableEntityManagementServiceBase<CatColor, CatColorDetailDto>(
            catColorApiClient,
            messagesProvider,
            photosDownloaderService,
            entityFormService,
            logger), ICatColorManagementService
    {
        private readonly ICatColorApiClient _catColorApiClient = catColorApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<CatColorManagementService> _logger = logger;

        protected override Func<CatColorDetailDto?, CatColor?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

        public async Task<ServiceResult<CatColor>> GetEntityAsync(string colorIdentifier, CancellationToken token = default)
        {
            var result = await _catColorApiClient.GetAsync(colorIdentifier.Replace(" ", ""), token);
            return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
        }

        public override async Task<ServiceResult<CatColor>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateCatColorDto(
                    Identifier: string.Empty,
                    Description: string.Empty);

                var result = await _catColorApiClient.AddAsync(dto, token);
                if (result != null)
                    return new(result.StatusCode, result.Message, result.Value?.ToEFModel());

                return new(500, _messagesProvider.CreateEntityAdditionErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить новый CatColor");
                return new(500, $"Не удалось добавить новый CatColor: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<PagedResult<CatColor>>> GetByPageAsync(
            int pageIndex,
            int pageSize,
            CancellationToken token = default)
        {
            try
            {
                var result = await _catColorApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    var page = result.Value!;
                    return new(result.StatusCode, null, new PagedResult<CatColor>(
                        [.. page.Items.Select(dto => dto.ToEFModel())],
                        page.PageNumber,
                        page.PageSize,
                        page.TotalItems,
                        page.TotalPages));
                }

                return new(500, _messagesProvider.CreateApiGetPageFailureMessage<CatColor>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу CatColor");
                return new(500, $"Не удалось получить страницу CatColor: {ex.Message}");
            }
        }
    }
}
