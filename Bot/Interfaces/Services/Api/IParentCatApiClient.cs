using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IParentCatApiClient : IDisplayableEntityApiClient
    {
        Task<ParentCatDetailDto?> AddAsync(CreateParentCatDto dto, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<ParentCatDetailDto?> GetAsync(int id, CancellationToken token = default);
        Task<ParentCatDetailDto?> UpdateAsync(int id, UpdateParentCatDto dto, CancellationToken token = default);

        Task<List<ParentCatListDto>?> GetListAsync(bool isMale = true, CancellationToken token = default);
        Task<List<ParentCatListDto>?> GetListAsync(CancellationToken token = default);
        Task<PagedResult<ParentCatMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default);
        Task<PagedResult<ParentCatMinimalDto>?> GetByPageAsync(bool isMale, int pageIndex, int pageSize, CancellationToken token = default);

        Task<FileUploadResult[]?> UploadGenTestsAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);
        Task<FileUploadResult[]?> UploadTitlesAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);

        Task<ParentCatDetailDto?> UpdateColorAsync(int entityId, string color, CancellationToken token);
    }
}