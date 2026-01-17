using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface ICatColorService
    {
        ServiceResult<PhotosLimitResponse> GetPhotosLimit(PhotosType photosType);
        Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> GetAsync(bool admin, string identifier, CancellationToken token = default);
        Task<ServiceResult<PagedResult<CatColorListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> UpdateAsync(int id, UpdateCatColorDto catColorDto, CancellationToken token = default);
        Task<ServiceResult<CatColorTree>> GetColorTreeAsync(CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<CatColorDetailDto>>> UploadFilesAsync(int id, string dictionaryName, IEnumerable<IFormFile> files, List<string>? telegramFileIds = null, CancellationToken token = default);
        Task<ServiceResult<CatColorDetailDto>> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token);
        Task<ServiceResult<CatColorDetailDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token);
    }
}