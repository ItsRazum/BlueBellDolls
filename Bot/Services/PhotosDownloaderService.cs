using BlueBellDolls.Bot.Adapters;
using BlueBellDolls.Bot.Interfaces;
using BlueBellDolls.Bot.Settings;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Bot.Services
{
    public class PhotosDownloaderService : IPhotosDownloaderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBotService _botService;
        private readonly TelegramFilesHttpClientSettings _telegramFilesHttpClientSettings;
        private readonly BotSettings _botSettings;

        public PhotosDownloaderService(
            IHttpClientFactory httpClientFactory,
            IBotService botService,
            IOptions<TelegramFilesHttpClientSettings> httpOptions,
            IOptions<BotSettings> botOptions)
        {
            _httpClientFactory = httpClientFactory;
            _botService = botService;
            _telegramFilesHttpClientSettings = httpOptions.Value;
            _botSettings = botOptions.Value;
        }

        public async Task<Dictionary<string, string>> DownloadAndConvertPhotosToBase64(PhotoAdapter[] photos, CancellationToken token)
        {
            var base64Photos = new Dictionary<string, string>();

            using var httpClient = _httpClientFactory.CreateClient(_telegramFilesHttpClientSettings.ClientName);

            foreach (var photoAdapter in photos)
            {
                var file = await _botService.GetFileAsync(photoAdapter.FileId, token);
                var fileUrl = $"https://api.telegram.org/file/bot{_botSettings.Token}/{file.FilePath}";

                var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync(token);
                base64Photos[photoAdapter.FileId] = await ConvertStreamToBase64Async(stream, token);
            }

            return base64Photos;
        }

        private async Task<string> ConvertStreamToBase64Async(Stream stream, CancellationToken token)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, token);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
