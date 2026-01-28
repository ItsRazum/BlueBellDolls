using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class FeedbackApiClient(IHttpClientFactory httpClientFactory) : TinyApiClientBase(httpClientFactory), IFeedbackApiClient
    {
        public async Task<ServiceResult<BookingRequestDetailDto>> CloseFeedbackRequestAsync(int requestId, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(
                $"api/admin/feedbackrequests/{requestId}/close",
                null,
                cancellationToken: token);

            return await FromResponse<BookingRequestDetailDto>(response, token);
        }
    }
}
