using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Services
{
    public class PhotosDownloaderService(
        IHttpClientFactory httpClientFactory,
        IBotService botService,
        IOptions<BotSettings> botOptions,
        ILogger<PhotosDownloaderService> logger) : IPhotosDownloaderService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IBotService _botService = botService;
        private readonly BotSettings _botSettings = botOptions.Value;
        private readonly ILogger<PhotosDownloaderService> _logger = logger;

        public async Task<(Stream PhotoStream, string FileName, string fileId)?> DownloadPhotoAsync(
        PhotoAdapter photo,
        CancellationToken token)
        {
            try
            {
                var file = await _botService.GetFileAsync(photo.FileId, token);
                if (file?.FilePath == null)
                {
                    _logger.LogWarning("Не удалось получить FilePath для FileId {FileId}", photo.FileId);
                    return null;
                }

                var fileUrl = $"https://api.telegram.org/file/bot{_botSettings.Token}/{file.FilePath}";

                using var httpClient = _httpClientFactory.CreateClient(Constants.TelegramHttpClientName);
                var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();

                await using var httpStream = await response.Content.ReadAsStreamAsync(token);
                var memoryStream = new MemoryStream();
                await httpStream.CopyToAsync(memoryStream, token);

                memoryStream.Position = 0;

                var fileName = Path.GetFileName(file.FilePath);

                return (memoryStream, fileName, photo.FileId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при скачивании файла {FileId} из Telegram", photo.FileId);
                return null;
            }
        }
    }
}
