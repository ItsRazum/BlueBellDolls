using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Api
{
    public interface IBookingApiClient
    {
        Task<ServiceResult<BookingRequestDetailDto>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<ServiceResult<BookingRequestDetailDto>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}