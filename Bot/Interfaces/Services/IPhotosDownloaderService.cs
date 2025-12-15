using BlueBellDolls.Bot.Adapters;

namespace BlueBellDolls.Bot.Interfaces.Services
{
    public interface IPhotosDownloaderService
    {
        Task<(Stream PhotoStream, string FileName, string fileId)?> DownloadPhotoAsync(PhotoAdapter photo, CancellationToken token);
    }
}