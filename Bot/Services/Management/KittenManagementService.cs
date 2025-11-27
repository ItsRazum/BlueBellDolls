using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Management
{
    public class KittenManagementService(
        IKittenApiClient kittenApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService, 
        IMessagesProvider messagesProvider,
        ILogger<KittenManagementService> logger) 
        : DisplayableEntityManagementServiceBase<Kitten>(
            kittenApiClient, 
            messagesProvider, 
            photosDownloaderService, 
            logger), ICatManagementService<Kitten>
    {
        private readonly IKittenApiClient _kittenApiClient = kittenApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<KittenManagementService> _logger = logger;

        public override async Task<Kitten?> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var dto = await _kittenApiClient.GetAsync(entityId, token);
            return dto?.ToEFModel();
        }

        public override Task<ManagementOperationResult<Kitten>> AddNewEntityAsync(CancellationToken token = default)
        {
            return Task.FromResult(new ManagementOperationResult<Kitten>(false, _messagesProvider.CreateKittenRequiresLitterMessage()));
        }

        public override async Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token)
        {
            try
            {
                var success = await _kittenApiClient.DeleteAsync(entityId, token);
                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<Kitten>> UpdateEntityByReplyAsync(
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token = default)
        {
            try
            {
                var currentDto = await _kittenApiClient.GetAsync(modelId, token);
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
                _logger.LogError(ex, "Не удалось обновить сущность Kitten {id} с помощью Reply!", modelId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<Kitten>> UpdateEntityAsync(Kitten entity, CancellationToken token = default)
        {
            try
            {
                var updateDto = entity.ToUpdateDto();
                var success = await _kittenApiClient.UpdateAsync(entity.Id, updateDto, token);
                if (!success)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                var finalDto = await _kittenApiClient.GetAsync(entity.Id, token);
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

        public override async Task<ManagementOperationResult<PagedResult<Kitten>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    return new(true, null, new PagedResult<Kitten>(
                        [.. result.Items.Select(dto => dto.ToEFModel())],
                        result.PageNumber,
                        result.PageSize,
                        result.TotalItems,
                        result.TotalPages));
                }

                return new(false, _messagesProvider.CreateApiGetPageFailureMessage<Kitten>());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<Kitten>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            try
            {
                var success = await _kittenApiClient.UpdateColorAsync(entityId, color, token);
                if (!success)
                    return new(false, _messagesProvider.CreateColorUpdateErrorMessage());

                var finalDto = await _kittenApiClient.GetAsync(entityId, token);
                if (finalDto == null)
                    return new(false, _messagesProvider.CreateApiGetEntityAfterUpdateFailureMessage());

                return new(true, null, finalDto.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
