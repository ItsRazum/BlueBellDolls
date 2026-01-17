using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface ILitterApiClient : IDisplayableEntityApiClient<LitterDetailDto>
    {
        Task<List<LitterDetailDto>?> GetListAsync(CancellationToken token = default);
        Task<LitterDetailDto?> AddAsync(CreateLitterDto dto, CancellationToken token = default);
        Task<KittenDetailDto?> AddKittenAsync(int litterId, CreateKittenDto newKitten, CancellationToken token = default);
        Task<LitterDetailDto?> SetMotherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<LitterDetailDto?> SetFatherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<PagedResult<LitterMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<LitterDetailDto?> UpdateAsync(int id, UpdateLitterDto dto, CancellationToken token = default);
    }
}