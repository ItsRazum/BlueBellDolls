using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public abstract class ApiClientBase<TDto>(
        IHttpClientFactory httpClientFactory,
        ILogger logger) : TinyApiClientBase(httpClientFactory) where TDto : class
    {
        private readonly ILogger _logger = logger;

        protected async Task<EntityFilesUploadResult<TDto>?> UploadFilesAsync(
            string requestUrl,
            IEnumerable<(Stream fileStream, string fileName, string fileId)> files,
            CancellationToken token)
        {
            if (files == null || !files.Any())
            {
                return null;
            }

            try
            {
                using var content = new MultipartFormDataContent();

                foreach (var (fileStream, fileName, _) in files)
                {
                    var streamContent = new StreamContent(fileStream);
                    content.Add(streamContent, "files", fileName);
                }

                foreach(var (_, fileName, fileId) in files)
                {
                    content.Add(new StringContent(fileId), "telegramFileIds");
                };

                var response = await HttpClient.PostAsync(requestUrl, content, token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<EntityFilesUploadResult<TDto>>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось загрузить файлы на сервер!");
                return null;
            }
        }
    }
}
