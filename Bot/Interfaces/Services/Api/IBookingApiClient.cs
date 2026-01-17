using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IBookingApiClient
    {
        Task<BookingRequestDetailDto?> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<BookingRequestDetailDto?> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}