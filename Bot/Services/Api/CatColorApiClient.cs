using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class CatColorApiClient(IHttpClientFactory httpClientFactory, ILogger<CatColorApiClient> logger)
        : DisplayableEntityApiClientBase<Kitten>(httpClientFactory, logger), ICatColorApiClient
    {
        private readonly ILogger<CatColorApiClient> _logger = logger;

        public async Task<CatColorDetailDto?> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<CatColorDetailDto>($"/api/admin/catcolors/{id}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь CatColor {id} с сервера!", nameof(CatColorApiClient), nameof(GetListAsync), id);
                return null;
            }
        }

        public async Task<CatColorDetailDto?> GetAsync(string colorIdentifier, CancellationToken token)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<CatColorDetailDto>($"/api/admin/catcolors/identifier/{colorIdentifier}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь CatColor {identifier} с сервера!", nameof(CatColorApiClient), nameof(GetAsync), colorIdentifier);
                return null;
            }
        }

        public async Task<List<CatColorDetailDto>?> GetListAsync(CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<List<CatColorDetailDto>>("/api/admin/catcolors", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список CatColor с сервера!", nameof(CatColorApiClient), nameof(GetListAsync));
                return null;
            }
        }

        public async Task<CatColorDetailDto?> AddAsync(CreateCatColorDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/catcolors", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CatColorDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность CatColor!", nameof(CatColorApiClient), nameof(AddAsync));
                return null;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateCatColorDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/catcolors/{id}", dto, token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить CatColor {id}!", nameof(CatColorApiClient), nameof(UpdateAsync), id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"/api/admin/catcolors/{id}", token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось удалить CatColor {id}!", nameof(CatColorApiClient), nameof(DeleteAsync), id);
                return false;
            }
        }

        public async Task<PagedResult<CatColorListDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<CatColorListDto>>($"/api/admin/catcolors?page={pageIndex}&pageSize={pageSize}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка CatColor!", nameof(CatColorApiClient), nameof(GetByPageAsync), pageIndex);
                return null;
            }
        }

        public async Task<CatColorTree?> GetCatColorTreeAsync(CancellationToken token)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<CatColorTree>("/api/admin/catcolors/tree", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить дерево CatColor!", nameof(CatColorApiClient), nameof(GetCatColorTreeAsync));
                return null;
            }
        }
    }
}
