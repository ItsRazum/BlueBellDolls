using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IConfigurationApiClient
    {
        Task<PhotosLimitsResponse?> GetPhotosLimitsAsync(CancellationToken token = default);
    }
}