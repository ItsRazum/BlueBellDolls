using BlueBellDolls.Bot.Interfaces.Services.Api.Base;
using BlueBellDolls.Bot.Services.Api;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Types
{
    public class DisplayableEntityApiClientBase<TEntity, TDto>(
        IHttpClientFactory httpClientFactory, 
        ILogger logger) 
        : ApiClientBase<TDto>(httpClientFactory, logger), IDisplayableEntityApiClient<TDto> where TEntity : class, IDisplayableEntity where TDto : class
    {
        private readonly ILogger _logger = logger;

        public virtual async Task<ServiceResult<PhotosLimitResponse>> GetPhotosLimitAsync(PhotosType photosType, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/photos?type={photosType}/limit", token);
                return await FromResponse<PhotosLimitResponse>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить лимит фотографий для сущности {type}!", typeof(TEntity).Name);
                return new(500, "Неизвестная ошибка");
            }

        }

        public virtual async Task<ServiceResult<TDto>> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}", token);
                return await FromResponse<TDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить сущность {type} {id}!", typeof(TEntity).Name, id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public virtual async Task<ServiceResult> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}", token);
                return await FromResponse(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось удалить Kitten {id}!", nameof(KittenApiClient), nameof(DeleteAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public virtual async Task<ServiceResult<TDto>> ToggleVisibilityAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}/toggle-visibility", null, token);
                return await FromResponse<TDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось переключить видимость сущности {type} {id}!", typeof(TEntity).Name, id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public virtual async Task<ServiceResult<EntityFilesUploadResult<TDto>>> UploadPhotosAsync(
                int id,
                IEnumerable<(Stream fileStream, string fileName, string fileId)> photos,
                CancellationToken token = default)
        {
            return await UploadFilesAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}/photos", photos, token);
        }

        public virtual async Task<ServiceResult<TDto>> SetDefaultPhotoAsync(int id, int photoId, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsync(
                    $"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}/photos/set-default?photoId={photoId}",
                    null,
                    token);
                return await FromResponse<TDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось установить фото по умолчанию для сущности {type} {id}!", typeof(TEntity).Name, id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public virtual async Task<ServiceResult<TDto>> DeletePhotosAsync(int id, int[] ids, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}/photos/delete-batch", ids, token);
                response.EnsureSuccessStatusCode();
                return await FromResponse<TDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить фотографии у сущности {type} {id}!", typeof(TEntity).Name, id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public virtual async Task<ServiceResult<TDto>> UpdateAsync(int id, Dictionary<string, string> body, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/{typeof(TEntity).Name.ToLower()}s/{id}", body, token);
                return await FromResponse<TDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обновить сущность {type} {id}!", typeof(TEntity).Name, id);
                return new(500, "Неизвестная ошибка");
            }
        }
    }
}
