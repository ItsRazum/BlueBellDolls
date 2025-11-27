using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
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
        : DisplayableEntityManagementServiceBase<Litter>(
            litterApiClient,
            messagesProvider,
            photosDownloaderService,
            logger), ILitterManagementService
    {
        private readonly ILitterApiClient _litterApiClient = litterApiClient;
        private readonly IParentCatApiClient _parentCatApiClient = parentCatApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<LitterManagementService> _logger = logger;

        public override async Task<Litter?> GetEntityAsync(int entityId, CancellationToken token = default)
        {
            var dto = await _litterApiClient.GetAsync(entityId, token);
            return dto?.ToEFModel();
        }

        public override async Task<ManagementOperationResult<Litter>> AddNewEntityAsync(CancellationToken token = default)
        {
            try
            {
                var dto = new CreateLitterDto(
                    BirthDay: DateOnly.FromDateTime(DateTime.Now),
                    Description: "Добавьте описание!",
                    MotherCatId: null,
                    FatherCatId: null
                );

                var resultDto = await _litterApiClient.AddAsync(dto, token);

                if (resultDto != null)
                    return new(true, null, resultDto.ToEFModel());

                return new(false, _messagesProvider.CreateEntityAdditionErrorMessage());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult> DeleteEntityAsync(int entityId, CancellationToken token)
        {
            try
            {
                var success = await _litterApiClient.DeleteAsync(entityId, token);
                if (success)
                    return new(true);

                return new(false, _messagesProvider.CreateEntityDeletionError());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<Litter>> UpdateEntityByReplyAsync(
            int modelId,
            Dictionary<string, string> properties,
            CancellationToken token = default)
        {
            try
            {
                var currentDto = await _litterApiClient.GetAsync(modelId, token);
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
                _logger.LogError(ex, "Ошибка при обновлении Litter {id}", modelId);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<Litter>> UpdateEntityAsync(Litter entity, CancellationToken token = default)
        {
            try
            {
                var updateDto = entity.ToUpdateDto();
                var success = await _litterApiClient.UpdateAsync(entity.Id, updateDto, token);
                if (!success)
                    return new(false, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                var finalDto = await _litterApiClient.GetAsync(entity.Id, token);
                if (finalDto == null)
                    return new(false, _messagesProvider.CreateApiGetEntityAfterUpdateFailureMessage());

                return new(true, null, finalDto.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить Litter {id}!", entity.Id);
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public override async Task<ManagementOperationResult<PagedResult<Litter>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _litterApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    return new(true, null, new PagedResult<Litter>(
                        [.. result.Items.Select(dto => dto.ToEFModel())],
                        result.PageNumber,
                        result.PageSize,
                        result.TotalItems,
                        result.TotalPages));
                }

                return new(false, _messagesProvider.CreateApiGetPageFailureMessage<Litter>());
            }
            catch (Exception ex)
            {
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult<Kitten>> AddNewKittenToLitterAsync(int litterId, CancellationToken token)
        {
            try
            {
                var kittenDto = new CreateKittenDto(
                    LitterId: litterId,
                    Name: "Новый котёнок",
                    IsMale: true,
                    Description: "Добавьте описание!",
                    Color: "Не указан",
                    Class: KittenClass.Pet,
                    Status: KittenStatus.Available);
                var result = await _litterApiClient.AddKittenAsync(litterId, kittenDto, token);
                return new(true, null, result?.ToEFModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(LitterManagementService), nameof(AddNewKittenToLitterAsync));
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ManagementOperationResult> SetParentCatForLitterAsync(int litterId, int parentCatId, CancellationToken token)
        {
            try
            {
                var parentCat = await _parentCatApiClient.GetAsync(parentCatId, token);
                if (parentCat != null)
                {
                    var result = parentCat.IsMale
                        ? await _litterApiClient.SetFatherCatAsync(litterId, parentCatId, token)
                        : await _litterApiClient.SetMotherCatAsync(litterId, parentCatId, token);

                    return new(true);
                }

                return new(false, _messagesProvider.CreateEntityNotFoundMessage(typeof(ParentCat), parentCatId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Service}.{Method}(): Произошла необработанная ошибка", nameof(LitterManagementService), nameof(SetParentCatForLitterAsync));
                return new(false, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
