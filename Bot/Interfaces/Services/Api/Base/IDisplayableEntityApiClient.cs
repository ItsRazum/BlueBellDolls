using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api.Base
{
    public interface IDisplayableEntityApiClient
    {
        Task<bool> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<FileUploadResult[]?> UploadPhotosAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default);
        Task<bool> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token = default);
        Task<bool> DeletePhotosAsync(int id, int[] ids, CancellationToken token = default);
    }
}
