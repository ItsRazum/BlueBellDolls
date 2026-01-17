using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Extensions;
using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Management.Base;
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
    public class KittenManagementService(
        IKittenApiClient kittenApiClient,
        IEntityFormService entityFormService,
        IPhotosDownloaderService photosDownloaderService,
        IMessagesProvider messagesProvider,
        ILogger<KittenManagementService> logger)
        : DisplayableEntityManagementServiceBase<Kitten, KittenDetailDto>(
            kittenApiClient,
            messagesProvider,
            photosDownloaderService,
            logger), IKittenManagementService
    {
        private readonly IKittenApiClient _kittenApiClient = kittenApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<KittenManagementService> _logger = logger;

        protected override Func<KittenDetailDto?, Kitten?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

        public override Task<ManagementOperationResult<Kitten>> AddNewEntityAsync(CancellationToken token = default)
        {
            return Task.FromResult(new ManagementOperationResult<Kitten>(false, _messagesProvider.CreateKittenRequiresLitterMessage()));
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
                var result = await _kittenApiClient.UpdateAsync(entity.Id, updateDto, token);
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
                var result = await _kittenApiClient.UpdateColorAsync(entityId, color, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateColorUpdateErrorMessage());

                return new(true, null, result.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<Kitten>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.UpdateStatusAsync(entityId, newStatus, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(true, null, result.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<Kitten>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.UpdateClassAsync(entityId, newClass, token);
                if (result == null)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(true, null, result.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
