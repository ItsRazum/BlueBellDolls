using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IKittenService
    {
        ServiceResult<PhotosLimitResponse> GetPhotosLimit(PhotosType photosType);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<KittenListDto>>> GetListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult<KittenListDto[]>> GetAvailableKittensAsync(CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateAsync(int id, UpdateKittenDto kittenDto, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateColorAsync(int id, string color, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateKittenClassAsync(int id, UpdateKittenClassRequest request, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> UpdateKittenStatusAsync(int id, UpdateKittenStatusRequest request, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<KittenDetailDto>>> UploadFilesAsync(int id, string dictionaryName, IEnumerable<IFormFile> files, List<string>? telegramFileIds = null, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token);
        Task<ServiceResult<KittenDetailDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token);
    }
}