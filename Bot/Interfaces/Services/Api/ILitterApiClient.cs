using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface ILitterApiClient : IDisplayableEntityApiClient<LitterDetailDto>
    {
        Task<ServiceResult<List<LitterDetailDto>>> GetListAsync(CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> AddAsync(CreateLitterDto dto, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> AddKittenAsync(int litterId, CreateKittenDto newKitten, CancellationToken token = default);
        Task<ServiceResult<SetParentCatForLitterResponse>> SetParentCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<ServiceResult<PagedResult<LitterMinimalDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
    }
}