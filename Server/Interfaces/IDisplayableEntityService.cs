using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Records;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IDisplayableEntityService
    {
        Task<ServiceResult> ToggleVisibilityAsync(int id, CancellationToken token = default);
        Task<ServiceResult<FileUploadResult[]>> UploadFilesAsync(int id, string dictionaryName, IEnumerable<IFormFile> files, List<string>? telegramFileIds = null, CancellationToken token = default);
        Task<ServiceResult> DeleteFilesAsync(int id, IEnumerable<int> photoIds, CancellationToken token);
        Task<ServiceResult> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token);
    }
}
