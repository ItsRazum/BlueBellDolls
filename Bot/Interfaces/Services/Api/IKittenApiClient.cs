using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IKittenApiClient : IDisplayableEntityApiClient
    {
        Task<KittenDetailDto?> AddAsync(CreateKittenDto dto, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<KittenDetailDto?> GetAsync(int id, CancellationToken token = default);
        Task<List<KittenListDto>?> GetListAsync(KittenStatus? status = null, CancellationToken token = default);
        Task<KittenDetailDto?> UpdateAsync(int id, UpdateKittenDto dto, CancellationToken token = default);
        Task<PagedResult<KittenMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<KittenDetailDto?> UpdateColorAsync(int entityId, string color, CancellationToken token);
    }
}