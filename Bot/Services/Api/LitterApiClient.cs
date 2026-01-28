using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class LitterApiClient(IHttpClientFactory httpClientFactory, ILogger<LitterApiClient> logger) 
        : DisplayableEntityApiClientBase<Litter, LitterDetailDto>(httpClientFactory, logger), ILitterApiClient
    {
        private readonly ILogger<LitterApiClient> _logger = logger;

        public async Task<ServiceResult<List<LitterDetailDto>>> GetListAsync(CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync("/api/admin/litters", token);
                return await FromResponse<List<LitterDetailDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось извлечь список Litter с сервера!", nameof(LitterApiClient), nameof(GetListAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<LitterDetailDto>> AddAsync(CreateLitterDto dto, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync("/api/admin/litters", dto, token);
                return await FromResponse<LitterDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить новую сущность Litter!", nameof(LitterApiClient), nameof(AddAsync));
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<KittenDetailDto>> AddKittenAsync(int litterId, CreateKittenDto newKitten, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"/api/admin/litters/{litterId}/kittens", newKitten, token);
                return await FromResponse<KittenDetailDto>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось добавить Kitten в Litter {id}!",
                    nameof(LitterApiClient), nameof(AddKittenAsync), litterId);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<SetParentCatForLitterResponse>> SetParentCatAsync(int litterId, int parentCatId, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.PutAsync($"/api/admin/litters/{litterId}/parent/{parentCatId}", null, token);
                return await FromResponse<SetParentCatForLitterResponse>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось установить маму ParentCat {parentCatId} для помёта {litterId}!", nameof(LitterApiClient), nameof(SetParentCatAsync), parentCatId, litterId);
                return new(500, "Неизвестная ошибка");
            }
        }

        public async Task<ServiceResult<PagedResult<LitterMinimalDto>>> GetByPageAsync(int pageIndex, int pageSize, CancellationToken token = default)
        {
            try
            {
                var response = await HttpClient.GetAsync($"/api/admin/litters?page={pageIndex}&pageSize={pageSize}", token);
                return await FromResponse<PagedResult<LitterMinimalDto>>(response, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось получить страницу {p} списка Litter!", nameof(LitterApiClient), nameof(GetByPageAsync), pageIndex);
                return new(500, "Неизвестная ошибка");
            }
        }
    }
}
