using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ICatColorApiClient : IDisplayableEntityApiClient
    {
        Task<CatColorDetailDto?> AddAsync(CreateCatColorDto dto, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<CatColorDetailDto?> GetAsync(int id, CancellationToken token = default);
        Task<PagedResult<CatColorListDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<List<CatColorDetailDto>?> GetListAsync(CancellationToken token = default);
        Task<bool> UpdateAsync(int id, UpdateCatColorDto dto, CancellationToken token = default);
    }
}