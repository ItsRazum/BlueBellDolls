using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IParentCatApiClient : IDisplayableEntityApiClient<ParentCatDetailDto>
    {
        Task<ServiceResult<ParentCatDetailDto>> AddAsync(CreateParentCatDto dto, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> UpdateAsync(int id, UpdateParentCatDto dto, CancellationToken token = default);

        Task<ServiceResult<List<ParentCatListDto>>> GetListAsync(bool isMale = true, CancellationToken token = default);
        Task<ServiceResult<List<ParentCatListDto>>> GetListAsync(CancellationToken token = default);
        Task<ServiceResult<PagedResult<ParentCatMinimalDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<ServiceResult<PagedResult<ParentCatMinimalDto>>> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default);

        Task<ServiceResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadGenTestsAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadTitlesAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);

        Task<ServiceResult<ParentCatDetailDto>> UpdateColorAsync(int entityId, string color, CancellationToken token);
    }
}