using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;

namespace BlueBellDolls.Bot.Services.Api
{
    public class BookingApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<BookingApiClient> logger) : ApiClientBase(httpClientFactory, logger), IBookingApiClient
    {
        public async Task<bool> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(
                $"api/admin/bookingrequests/{bookingId}/process?telegramUserId={telegramUserId}",
                null,
                cancellationToken: token);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(
                $"api/admin/bookingrequests/{bookingId}/close?telegramUserId={telegramUserId}",
                null,
                cancellationToken: token);

            return response.IsSuccessStatusCode;
        }
    }
}
