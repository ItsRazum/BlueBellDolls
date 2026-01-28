using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Types;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Services.Api
{
    public class BookingApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<BookingApiClient> logger) : ApiClientBase<BookingRequestDetailDto>(httpClientFactory, logger), IBookingApiClient
    {
        public async Task<ServiceResult<BookingRequestDetailDto>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(
                $"api/admin/bookingrequests/{bookingId}/process?telegramUserId={telegramUserId}",
                null,
                cancellationToken: token);

            return await FromResponse<BookingRequestDetailDto>(response, token);
        }

        public async Task<ServiceResult<BookingRequestDetailDto>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var response = await HttpClient.PostAsync(
                $"api/admin/bookingrequests/{bookingId}/close?telegramUserId={telegramUserId}",
                null,
                cancellationToken: token);

            return await FromResponse<BookingRequestDetailDto>(response, token);
        }
    }
}
