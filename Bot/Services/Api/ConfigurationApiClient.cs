using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class ConfigurationApiClient(IHttpClientFactory httpClientFactory) : TinyApiClientBase(httpClientFactory), IConfigurationApiClient
    {
        public async Task<PhotosLimitsResponse?> GetPhotosLimitsAsync(CancellationToken token)
        {
            var result = await HttpClient.GetAsync("admin/configuration/photos/limit", token);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<PhotosLimitsResponse>(token);
        }
    }
}
