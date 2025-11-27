using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface ILitterApiClient : IDisplayableEntityApiClient
    {
        Task<LitterDetailDto?> GetAsync(int id, CancellationToken token = default);
        Task<List<LitterDetailDto>?> GetListAsync(CancellationToken token = default);
        Task<LitterDetailDto?> AddAsync(CreateLitterDto dto, CancellationToken token = default);
        Task<KittenDetailDto?> AddKittenAsync(int litterId, CreateKittenDto newKitten, CancellationToken token = default);
        Task<bool> SetMotherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<bool> SetFatherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<PagedResult<LitterMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<bool> UpdateAsync(int id, UpdateLitterDto dto, CancellationToken token = default);
    }
}