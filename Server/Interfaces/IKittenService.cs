using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IKittenService : IDisplayableEntityService
    {
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> GetAsync(int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<KittenListDto>>> GetListAsync(int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateKittenDto kittenDto, CancellationToken token = default);

        Task<ServiceResult> UpdateColorAsync(int id, string color, CancellationToken token = default);
    }
}