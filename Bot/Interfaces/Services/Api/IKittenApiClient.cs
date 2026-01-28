using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IKittenApiClient : IDisplayableEntityApiClient<KittenDetailDto>
    {
        Task<ServiceResult<KittenDetailDto>> AddAsync(CreateKittenDto dto, CancellationToken token = default);
        Task<ServiceResult<List<KittenListDto>>> GetListAsync(KittenStatus? status = null, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateAsync(int id, UpdateKittenDto dto, CancellationToken token = default);
        Task<ServiceResult<PagedResult<KittenMinimalDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateColorAsync(int entityId, string color, CancellationToken token);
        Task<ServiceResult<KittenDetailDto>> UpdateStatusAsync(int entityId, KittenStatus newStatus, CancellationToken token);
        Task<ServiceResult<KittenDetailDto>> UpdateClassAsync(int entityId, KittenClass newClass, CancellationToken token);
    }
}