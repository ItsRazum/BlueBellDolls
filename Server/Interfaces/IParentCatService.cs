using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IParentCatService
    {
        ServiceResult<PhotosLimitResponse> GetPhotosLimit(PhotosType photosType);
        Task<ServiceResult<ParentCatDetailDto>> AddAsync(CreateParentCatDto parentCatDto, CancellationToken token = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<ParentCatListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, bool? isMale, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> UpdateAsync(int id, UpdateParentCatDto parentCatDto, CancellationToken token = default);

        Task<ServiceResult<ParentCatDetailDto>> UpdateColorAsync(int id, string color, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadFilesAsync(int id, string dictionaryName, IEnumerable<IFormFile> files, List<string>? telegramFileIds = null, CancellationToken token = default);
        Task<ServiceResult<ParentCatDetailDto>> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token);
        Task<ServiceResult<ParentCatDetailDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token);

    }
}