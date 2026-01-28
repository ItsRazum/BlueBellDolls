using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Bot.Interfaces.Services.Api;

namespace BlueBellDolls.Bot.Services.Api
{
    public class ParentCatApiClient(IHttpClientFactory httpClientFactory, ILogger<ParentCatApiClient> logger) 
        : DisplayableEntityApiClientBase<ParentCat, ParentCatDetailDto>(httpClientFactory, logger), IParentCatApiClient
    {
        private readonly ILogger<ParentCatApiClient> _logger = logger;

        public async Task<ServiceResult<List<ParentCatListDto>>> GetListAsync(CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/admin/parentcats", token);
                return await FromResponse<List<ParentCatListDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список ParentCat с сервера!", nameof(ParentCatApiClient), nameof(GetListAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<List<ParentCatListDto>>> GetListAsync(bool isMale = true, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/parentcats?isMale={isMale}", token);
                return await FromResponse<List<ParentCatListDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список ParentCat с сервера! (isMale={isMale})", nameof(ParentCatApiClient), nameof(GetListAsync), isMale);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> AddAsync(CreateParentCatDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/parentcats", dto, token);
                return await FromResponse<ParentCatDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность ParentCat!", nameof(ParentCatApiClient), nameof(AddAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> UpdateAsync(int id, UpdateParentCatDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/parentcats/{id}", dto, token);
                return await FromResponse<ParentCatDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить ParentCat {id}!", nameof(ParentCatApiClient), nameof(UpdateAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadTitlesAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default)
        {
            try
            {
                return await UploadFilesAsync($"/api/admin/parentcats/{id}/titles", photos, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось загрузить титулы для ParentCat {id}!", nameof(ParentCatApiClient), nameof(UploadTitlesAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadGenTestsAsync(int id, IEnumerable<(Stream fileStream, string fileName, string fileId)> photos, CancellationToken token = default)
        {
            try
            {
                return await UploadFilesAsync($"/api/admin/parentcats/{id}/gentests", photos, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось загрузить генетические тесты для ParentCat {id}!", nameof(ParentCatApiClient), nameof(UploadGenTestsAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<PagedResult<ParentCatMinimalDto>>> GetByPageAsync(int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/parentcats?page={pageNumber}&pageSize={pageSize}", token);
                return await FromResponse<PagedResult<ParentCatMinimalDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка ParentCat!", nameof(ParentCatApiClient), nameof(GetByPageAsync), pageNumber);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<PagedResult<ParentCatMinimalDto>>> GetByPageAsync(bool isMale, int pageNumber, int pageSize, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/parentcats?isMale={isMale}&page={pageNumber}&pageSize={pageSize}", token);
                return await FromResponse<PagedResult<ParentCatMinimalDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка ParentCat!", nameof(ParentCatApiClient), nameof(GetByPageAsync), pageNumber);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<ParentCatDetailDto>> UpdateColorAsync(int entityId, string color, CancellationToken token)
        {
            var requestUrl = $"/api/admin/parentcats/{entityId}/color";
            try
            {
                var response = await HttpClient.PutAsJsonAsync(requestUrl, new { Color = color }, token);
                return await FromResponse<ParentCatDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить цвет для ParentCat {id}!",
                    nameof(ParentCatApiClient), nameof(UpdateColorAsync), entityId);
                return new(500, "Неизвестная ошибка");
            }
        }
    }
}
