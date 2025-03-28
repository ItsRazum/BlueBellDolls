using BlueBellDolls.Bot.Adapters;

namespace BlueBellDolls.Bot.Interfaces
{
    public interface IPhotosDownloaderService
    {
        Task<Dictionary<string, string>> DownloadAndConvertPhotosToBase64(PhotoAdapter[] photos, CancellationToken token);
    }
}