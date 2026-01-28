using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;

namespace BlueBellDolls.Bot.Interfaces.Services.Management
{
    public interface IBookingManagementService
    {
        Task<ServiceResult<BookingRequest>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
        Task<ServiceResult<BookingRequest>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default);
    }
}