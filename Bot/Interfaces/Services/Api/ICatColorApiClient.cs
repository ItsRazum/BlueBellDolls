using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface ICatColorApiClient : IDisplayableEntityApiClient<CatColorDetailDto>
    {
        Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> GetAsync(string colorIdentifier, CancellationToken token);

        Task<ServiceResult<PagedResult<CatColorListDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<ServiceResult<CatColorTree>> GetCatColorTreeAsync(CancellationToken token);
        Task<ServiceResult<List<CatColorDetailDto>>> GetListAsync(CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> UpdateAsync(int id, UpdateCatColorDto dto, CancellationToken token = default);
    }
}