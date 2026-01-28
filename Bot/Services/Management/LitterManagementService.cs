using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces.Services;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Services.Management;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Management
{
    public class LitterManagementService(
        ILitterApiClient litterApiClient,
        IParentCatApiClient parentCatApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IMessagesProvider messagesProvider,
        ILogger<LitterManagementService> logger)
        : DisplayableEntityManagementServiceBase<Litter, LitterDetailDto>(
            litterApiClient,
            messagesProvider,
            photosDownloaderService,
            entityFormService,
            logger), ILitterManagementService
    {
        private readonly ILitterApiClient _litterApiClient = litterApiClient;
        private readonly IParentCatApiClient _parentCatApiClient = parentCatApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<LitterManagementService> _logger = logger;

        protected override Func<LitterDetailDto?, Litter?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

        public override async Task<ServiceResult<Litter>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateLitterDto(
                    BirthDay: DateOnly.FromDateTime(DateTime.Now),
                    Description: string.Empty,
                    MotherCatId: null,
                    FatherCatId: null
                );

                var result = await _litterApiClient.AddAsync(dto, token);

                if (result != null)
                    return new(result.StatusCode, result.Message, result.Value?.ToEFModel());

                return new(500, _messagesProvider.CreateEntityAdditionErrorMessage());
            }
            catch (Exception ex)
            {
                return new(500, $"Не удалось добавить помет: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<PagedResult<Litter>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _litterApiClient.GetByPageAsync(pageIndex, pageSize, token);

                var page = result.Value;
                if (page == null)
                    return new(result.StatusCode, result.Message);

                return new(result.StatusCode, result.Message, new PagedResult<Litter>(
                    [.. page.Items.Select(dto => dto.ToEFModel())],
                    page.PageNumber,
                    page.PageSize,
                    page.TotalItems,
                    page.TotalPages));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу Litter!");
                return new(500, $"Не удалось получить страницу: {ex.Message}");
            }
        }

        public async Task<ServiceResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token)
        {
            try
            {
                var kittenDto = new CreateKittenDto(
                    LitterId: litterId,
                    Name: string.Empty,
                    IsMale: true,
                    Description: string.Empty,
                    Class: KittenClass.Pet,
                    Status: KittenStatus.Available);
                var result = await _litterApiClient.AddKittenAsync(litterId, kittenDto, token);
                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(LitterManagementService), nameof(AddNewKittenToLitterAsync));
                return new(500, $"Не удалось добавить котенка: {ex.Message}");
            }
        }

        public async Task<ServiceResult<StructWrapper<(bool isMale, Litter litter)>>> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token)
        {
            try
            {
                var result = await _litterApiClient.SetParentCatAsync(litterId, parentCatId, token);
                if (result.Value == null)
                    return new(result.StatusCode, result.Message);

                return new(result.StatusCode, result.Message, new((result.Value.IsMale, result.Value.Litter.ToEFModel())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(LitterManagementService), nameof(SetParentCatForLitterAsync));
                return new(500, $"Не удалось установить родителя: {ex.Message}");
            }
        }
    }
}
