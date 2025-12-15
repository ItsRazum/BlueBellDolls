using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IParentCatService : IDisplayableEntityService
    {

        Task<ServiceResult<ParentCatDetailDto>> AddAsync(CreateParentCatDto parentCatDto, CancellationToken token = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<ParentCatListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, bool? isMale, CancellationToken token = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateParentCatDto parentCatDto, CancellationToken token = default);

        Task<ServiceResult> UpdateColorAsync(int id, string color, CancellationToken token = default);

    }
}