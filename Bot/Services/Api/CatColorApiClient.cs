using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class CatColorApiClient(IHttpClientFactory httpClientFactory, ILogger<CatColorApiClient> logger)
        : DisplayableEntityApiClientBase<CatColor, CatColorDetailDto>(httpClientFactory, logger), ICatColorApiClient
    {
        private readonly ILogger<CatColorApiClient> _logger = logger;

        public async Task<ServiceResult<CatColorDetailDto>> GetAsync(string colorIdentifier, CancellationToken token)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/catcolors/identifier/{colorIdentifier}", token);
                return await FromResponse<CatColorDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь CatColor {identifier} с сервера!", nameof(CatColorApiClient), nameof(GetAsync), colorIdentifier);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<List<CatColorDetailDto>>> GetListAsync(CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/admin/catcolors", token);
                return await FromResponse<List<CatColorDetailDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список CatColor с сервера!", nameof(CatColorApiClient), nameof(GetListAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> AddAsync(CreateCatColorDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/catcolors", dto, token);
                return await FromResponse<CatColorDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность CatColor!", nameof(CatColorApiClient), nameof(AddAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<CatColorDetailDto>> UpdateAsync(int id, UpdateCatColorDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsJsonAsync($"/api/admin/catcolors/{id}", dto, token);
                return await FromResponse<CatColorDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось обновить CatColor {id}!", nameof(CatColorApiClient), nameof(UpdateAsync), id);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<PagedResult<CatColorListDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/catcolors?page={pageIndex}&pageSize={pageSize}", token);
                return await FromResponse<PagedResult<CatColorListDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка CatColor!", nameof(CatColorApiClient), nameof(GetByPageAsync), pageIndex);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<CatColorTree>> GetCatColorTreeAsync(CancellationToken token)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/admin/catcolors/tree", token);
                return await FromResponse<CatColorTree>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить дерево CatColor!", nameof(CatColorApiClient), nameof(GetCatColorTreeAsync));
                return new(500, "Неизвестная ошибка");
            }
        }
    }
}
