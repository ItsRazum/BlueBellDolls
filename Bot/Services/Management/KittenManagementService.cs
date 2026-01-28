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
            entityFormService,
            logger), IKittenManagementService
    {
        private readonly IKittenApiClient _kittenApiClient = kittenApiClient;
        private readonly IEntityFormService _entityFormService = entityFormService;
        private readonly IMessagesProvider _messagesProvider = messagesProvider;
        private readonly ILogger<KittenManagementService> _logger = logger;

        protected override Func<KittenDetailDto?, Kitten?> DtoToEntityFunc => (dto) => dto?.ToEFModel();

        public override Task<ServiceResult<Kitten>> AddNewEntityAsync(CancellationToken token = default)
        {
            return Task.FromResult(new ServiceResult<Kitten>(403, _messagesProvider.CreateKittenRequiresLitterMessage()));
        }

        public override async Task<ServiceResult<PagedResult<Kitten>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.GetByPageAsync(pageIndex, pageSize, token);

                if (result != null)
                {
                    var page = result.Value;
                    return new(result.StatusCode, result.Message, new PagedResult<Kitten>(
                        [.. page!.Items.Select(dto => dto.ToEFModel())],
                        page.PageNumber,
                        page.PageSize,
                        page.TotalItems,
                        page.TotalPages));
                }

                return new(500, _messagesProvider.CreateApiGetPageFailureMessage<Kitten>());
            }
            catch (Exception ex)
            {
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ServiceResult<Kitten>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            try
            {
                var result = await _kittenApiClient.UpdateColorAsync(entityId, color, token);
                if (result == null)
                    return new(500, _messagesProvider.CreateColorUpdateErrorMessage());

                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ServiceResult<Kitten>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.UpdateStatusAsync(entityId, newStatus, token);
                if (result == null)
                    return new(500, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }

        public async Task<ServiceResult<Kitten>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token = default)
        {
            try
            {
                var result = await _kittenApiClient.UpdateClassAsync(entityId, newClass, token);
                if (result == null)
                    return new(500, _messagesProvider.CreateApiUpdateEntityFailureMessage());

                return new(result.StatusCode, result.Message, result.Value?.ToEFModel());
            }
            catch (Exception ex)
            {
                return new(500, _messagesProvider.CreateUnknownErrorMessage(ex.Message));
            }
        }
    }
}
