using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api.Base
{
    public interface IDisplayableEntityApiClient<TDto> where TDto : class
    {
        Task<PhotosLimitResponse?> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default);
        Task<TDto?> GetAsync(int id, CancellationToken token = default);
        Task<bool> DeleteAsync(int id, CancellationToken token = default);
        Task<TDto?> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<EntityFilesUploadResult<TDto>?> UploadPhotosAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);
        Task<TDto?> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token = default);
        Task<TDto?> DeletePhotosAsync(int id, int[] ids, CancellationToken token = default);
    }
}
