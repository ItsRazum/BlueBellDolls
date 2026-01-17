using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface ILitterService
    {
        ServiceResult<PhotosLimitResponse> GetPhotosLimit(PhotosType photosType);
        Task<ServiceResult<LitterDetailDto>> AddAsync(CreateLitterDto litterDto, CancellationToken token = default);
        Task<ServiceResult<KittenDetailDto>> AddKittenToLitter(int litterId, CreateKittenDto kittenDto, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> SetFatherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> SetMotherCatAsync(int litterId, int parentCatId, CancellationToken token = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> GetAsync(bool admin, int id, CancellationToken token = default);
        Task<ServiceResult<PagedResult<LitterDetailDto>>> GetDetailListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult<PagedResult<LitterMinimalDto>>> GetMinimalListAsync(bool admin, int pageNumber, int pageSize, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> UpdateAsync(int id, UpdateLitterDto litterDto, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<LitterDetailDto>>> UploadFilesAsync(int id, string dictionaryName, IEnumerable<IFormFile> files, List<string>? telegramFileIds = null, CancellationToken token = default);
        Task<ServiceResult<LitterDetailDto>> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token);
        Task<ServiceResult<LitterDetailDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token);
    }
}