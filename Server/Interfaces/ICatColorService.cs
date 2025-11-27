using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface ICatColorService : IDisplayableEntityService
    {
        Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> GetAsync(int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<CatColorListDto>>> GetListAsync(int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult> UpdateAsync(int id, UpdateCatColorDto catColorDto, CancellationToken token = default);
    }
}