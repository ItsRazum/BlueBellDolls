using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public class DisplayableEntityApiClientBase<TEntity>(
        IHttpClientFactory httpClientFactory, 
        ILogger logger) 
        : ApiClientBase(httpClientFactory, logger), IDisplayableEntityApiClient where TEntity : class, IDisplayableEntity
    {
        private readonly ILogger _logger = logger;

        public virtual async Task<bool> ToggleVisibilityAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsync($"/api/{typeof(TEntity).Name.ToLower()}s/{id}/toggle-visibility", null, token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось переключить видимость сущности {type} {id}!", typeof(TEntity).Name, id);
                return false;
            }
        }

        public virtual async Task<FileUploadResult[]?> UploadPhotosAsync(
                int id,
                IEnumerable<(Stream fileStream, string fileName, string fileId)> photos,
                CancellationToken token = default)
        {
            return await UploadFilesAsync($"/api/{typeof(TEntity).Name.ToLower()}s/{id}/photos", photos, token);
        }


        public virtual async Task<bool> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsync(
                    $"/api/{typeof(TEntity).Name.ToLower()}s/{id}/photos/set-default?photoId={photoId}",
                    null,
                    token);

                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить фото по умолчанию для сущности {type} {id}!", typeof(TEntity).Name, id);
                return false;
            }
        }

        public virtual async Task<bool> DeletePhotosAsync(int id, int[] ids, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"/api/{typeof(TEntity).Name.ToLower()}s/{id}/photos/delete-batch", ids, token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить фотографии у сущности {type} {id}!", typeof(TEntity).Name, id);
                return false;
            }
        }
    }
}
