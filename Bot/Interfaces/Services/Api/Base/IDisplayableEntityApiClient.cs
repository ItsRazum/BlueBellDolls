using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api.Base
{
    public interface IDisplayableEntityApiClient<TDto> where TDto : class
    {
        Task<ServiceResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default);
        Task<ServiceResult<TDto>> GetAsync(int id, CancellationToken token = default);
        Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default);
        Task<ServiceResult<TDto>> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<EntityFilesUploadResult<TDto>>> UploadPhotosAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);
        Task<ServiceResult<TDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token = default);
        Task<ServiceResult<TDto>> DeletePhotosAsync(int id, int[] ids, CancellationToken token = default);
        Task<ServiceResult<TDto>> UpdateAsync(int id, Dictionary<string, string> dto, CancellationToken token = default);
    }
}
