using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using System.Net.Http.Json;

namespace BlueBellDolls.Bot.Services.Api
{
    public class LitterApiClient(IHttpClientFactory httpClientFactory, ILogger<LitterApiClient> logger) 
        : DisplayableEntityApiClientBase<Litter>(httpClientFactory, logger), ILitterApiClient
    {
        private readonly ILogger<LitterApiClient> _logger = logger;

        public async Task<LitterDetailDto?> GetAsync(int id, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<LitterDetailDto>($"/api/admin/litters/{id}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь Litter {id} с сервера!", nameof(LitterApiClient), nameof(GetListAsync), id);
                return null;
            }
        }

        public async Task<List<LitterDetailDto>?> GetListAsync(CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<List<LitterDetailDto>>("/api/admin/litters", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список Litter с сервера!", nameof(LitterApiClient), nameof(GetListAsync));
                return null;
            }
        }

        public async Task<LitterDetailDto?> AddAsync(CreateLitterDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/litters", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LitterDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность Litter!", nameof(LitterApiClient), nameof(AddAsync));
                return null;
            }
        }

        public async Task<KittenDetailDto?> AddKittenAsync(int litterId, CreateKittenDto newKitten, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"/api/admin/litters/{litterId}/kittens", newKitten, token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<KittenDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить Kitten в Litter {id}!",
                    nameof(LitterApiClient), nameof(AddKittenAsync), litterId);
                return null;
            }
        }

        public async Task<LitterDetailDto?> SetMotherCatAsync(int litterId, int motherCatId, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsync($"/api/admin/litters/{litterId}/mother/{motherCatId}", null, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LitterDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось установить маму ParentCat {parentCatId} для помёта {litterId}!", nameof(LitterApiClient), nameof(UpdateAsync), motherCatId, litterId);
                return null;
            }
        }

        public async Task<LitterDetailDto?> SetFatherCatAsync(int litterId, int fatherCatId, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsync($"/api/admin/litters/{litterId}/father/{fatherCatId}", null, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LitterDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось установить папу ParentCat {parentCatId} для помёта {litterId}!", nameof(LitterApiClient), nameof(UpdateAsync), fatherCatId, litterId);
                return null;
            }
        }

        public async Task<LitterDetailDto?> UpdateAsync(int id, UpdateLitterDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/litters/{id}", dto, token);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<LitterDetailDto>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить Litter {id}!", nameof(LitterApiClient), nameof(UpdateAsync), id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"/api/admin/litters/{id}", token);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось удалить Litter {id}!", nameof(LitterApiClient), nameof(DeleteAsync), id);
                return false;
            }
        }

        public async Task<PagedResult<LitterMinimalDto>?> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<PagedResult<LitterMinimalDto>>($"/api/admin/litters?page={pageIndex}&pageSize={pageSize}", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка Litter!", nameof(LitterApiClient), nameof(GetByPageAsync), pageIndex);
                return null;
            }
        }
    }
}
