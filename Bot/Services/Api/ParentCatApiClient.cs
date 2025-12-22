using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Bot.Interfaces.Services.Api;

namespace BlueBellDolls.Bot.Services.Api
{
    public class ParentCatApiClient(IHttpClientFactory httpClientFactory, ILogger<ParentCatApiClient> logger) 
        : DisplayableEntityApiClientBase<ParentCat>(httpClientFactory, logger), IParentCatApiClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(Constants.BlueBellDollsHttpClientName);
        private readonly ILogger<ParentCatApiClient> _logger = logger;

        public async Task<List<ParentCatListDto>?> GetListAsync(CancellationToken token = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ParentCatListDto>>($"api/admin/parentcats", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список ParentCat с сервера!", nameof(ParentCatApiClient), nameof(GetListAsync));
                return null;
            }
        }

        public async Task<List<ParentCatListDto>?> GetListAsync(bool isMale = true, CancellationToken token = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ParentCatListDto>>($"api/admin/parentcats?isMale={isMale}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь списот ParentCat с сервера! (isMale={isMale})", nameof(ParentCatApiClient), nameof(GetListAsync), isMale);
                return null;
            }
        }

        public async Task<ParentCatDetailDto?> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ParentCatDetailDto>($"/api/admin/parentcats/{id}", cancellationToken: token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь ParentCat {id} с сервера!", nameof(ParentCatApiClient), nameof(GetAsync), id);
                return null;
            }
        }

        public async Task<ParentCatDetailDto?> AddAsync(CreateParentCatDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/admin/parentcats", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ParentCatDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность ParentCat!", nameof(ParentCatApiClient), nameof(AddAsync));
                return null;
            }
        }

        public async Task<ParentCatDetailDto?> UpdateAsync(int id, UpdateParentCatDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/admin/parentcats/{id}", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ParentCatDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить ParentCat {id}!", nameof(ParentCatApiClient), nameof(UpdateAsync), id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/admin/parentcats/{id}", token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось удалить ParentCat {id}!", nameof(ParentCatApiClient), nameof(DeleteAsync), id);
                return false;
            }
        }

        public async Task<FileUploadResult[]?> UploadTitlesAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default)
        {
            return await UploadFilesAsync($"/api/admin/parentcats/{id}/titles", photos, token);
        }

        public async Task<FileUploadResult[]?> UploadGenTestsAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default)
        {
            return await UploadFilesAsync($"/api/admin/parentcats/{id}/gentests", photos, token);
        }

        public async Task<PagedResult<ParentCatMinimalDto>?> GetByPageAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<ParentCatMinimalDto>>($"/api/admin/parentcats?page={pageNumber}&pageSize={pageSize}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка ParentCat!", nameof(ParentCatApiClient), nameof(GetByPageAsync), pageNumber);
                return null;
            }
        }

        public async Task<PagedResult<ParentCatMinimalDto>?> GetByPageAsync(bool isMale, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<ParentCatMinimalDto>>($"/api/admin/parentcats?isMale={isMale}&page={pageNumber}&pageSize={pageSize}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка ParentCat!", nameof(ParentCatApiClient), nameof(GetByPageAsync), pageNumber);
                return null;
            }
        }

        public async Task<ParentCatDetailDto?> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            var requestUrl = $"/api/admin/parentcats/{entityId}/color";
            try
            {
                var response = await HttpClient.PutAsJsonAsync(requestUrl, new { Color = color }, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ParentCatDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить цвет для ParentCat {id}!",
                    nameof(KittenApiClient), nameof(UpdateColorAsync), entityId);
                return null;
            }
        }
    }
}
