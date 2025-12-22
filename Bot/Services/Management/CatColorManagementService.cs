using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Services;

namespace BlueBellDolls.Bot.Services.Management
{
    public class CatColorManagementService(
        ICatColorApiClient catColorApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IMessagesProvider messagesProvider,
        ILogger<CatColorManagementService> logger)
        : DisplayableEntityManagementServiceBase<CatColor>(
            catColorApiClient,
            messagesProvider,
            photosDownloaderService,
            logger), ICatColorManagementService
    {
        private readonly ICatColorApiClient _catColorApiClient = catColorApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<CatColorManagementService> _logger = logger;

        public override async Task<CatColor?> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var dto = await _catColorApiClient.GetAsync(entityId, token);
            return dto?.ToEFModel();
        }

        public async Task<CatColor?> GetEntityAsync(string colorIdentifier, CancellationToken token = default)
        {
            var dto = await _catColorApiClient.GetAsync(colorIdentifier, token);
            return dto?.ToEFModel();
        }

        public override async Task<ManagementOperationResult<CatColor>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateCatColorDto(
                    Identifier: "Новый окрас",
                    Description: "Добавьте описание!");

                var result = await _catColorApiClient.AddAsync(dto, token);
                if (result != null)
                    return new(true, null, result.ToEFModel());

                return new(false, _messagesProvider.CreateEntityAdditionErrorMessage());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить новый CatColor");
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token = default)
        {
            try
            {
                var success = await _catColorApiClient.DeleteAsync(entityId, token);
                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить CatColor {id}", entityId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<CatColor>> UpdateEntityAsync(CatColor entity, CancellationToken token = default)
        {
            try
            {
                var updateDto = entity.ToUpdateDto();
                var result = await _catColorApiClient.UpdateAsync(entity.Id, updateDto, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(true, null, result.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить CatColor {id}!", entity.Id);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<CatColor>> UpdateEntityByReplyAsync(
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token = default)
        {
            try
            {
                var currentDto = await _catColorApiClient.GetAsync(modelId, token);
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
                _logger.LogError(ex, "Ошибка при обновлении CatColor {id} через Reply", modelId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<PagedResult<CatColor>>> GetByPageAsync(
            int pageIndex,
            int pageSize,
            CancellationToken token = default)
        {
            try
            {
                var result = await _catColorApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    return new(true, null, new PagedResult<CatColor>(
                        [.. result.Items.Select(dto => dto.ToEFModel())],
                        result.PageNumber,
                        result.PageSize,
                        result.TotalItems,
                        result.TotalPages));
                }

                return new(false, _messagesProvider.CreateApiGetPageFailureMessage<CatColor>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить страницу CatColor");
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
