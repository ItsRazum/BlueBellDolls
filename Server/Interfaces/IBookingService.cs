using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Server.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResult<BookingRequestDetailDto>> AddBookingRequestAsync(CreateBookingRequestDto dto, CancellationToken token = default);

        Task<ServiceResult<BookingRequestDetailDto>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);

        Task<ServiceResult<BookingRequestDetailDto>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}
