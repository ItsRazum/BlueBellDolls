using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface ILitterService : IDisplayableEntityService
    {
        Task<ServiceResult<LitterDetailDto>> AddAsync(CreateLitterDto litterDto, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> AddKittenToLitter(int litterId, CreateKittenDto kittenDto, CancellationToken token = default);
        Task<ServiceResult> SetFatherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<ServiceResult> SetMotherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<LitterDetailDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateLitterDto litterDto, CancellationToken token = default);
    }
}