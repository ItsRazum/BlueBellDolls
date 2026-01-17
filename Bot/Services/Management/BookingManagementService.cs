using BlueBellDolls.Bot.Interfaces.Management;
using BlueBellDolls.Bot.Interfaces.Services.Api;
using BlueBellDolls.Bot.Records;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Models;

namespace BlueBellDolls.Bot.Services.Management
{
    public class BookingManagementService(
        IBookingApiClient bookingApiClient,
        ILogger<BookingManagementService> logger) : IBookingManagementService
    {
        private readonly IBookingApiClient _bookingApiClient = bookingApiClient;
        private readonly ILogger<BookingManagementService> _logger = logger;

        public async Task<ManagementOperationResult<BookingRequest>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var result = await _bookingApiClient.ProcessBookingRequestAsync(bookingId, telegramUserId, token);

                return new()
                {
                    Success = result != null,
                    Result = result?.ToEFModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось сменить статус брони {bookingId}", nameof(BookingManagementService), nameof(ProcessBookingRequestAsync), bookingId);
                return new()
                {
                    Success = false,
                    ErrorText = ex.Message
                };
            }
        }

        public async Task<ManagementOperationResult<BookingRequest>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var result = await _bookingApiClient.CloseBookingRequestAsync(bookingId, telegramUserId, token);
                return new()
                {
                    Success = result != null,
                    Result = result?.ToEFModel()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{service}.{method}(): Не удалось закрыть бронь {bookingId}", nameof(BookingManagementService), nameof(CloseBookingRequestAsync), bookingId);
                return new()
                {
                    Success = false,
                    ErrorText = $"Не удалось закрыть бронь {bookingId}: {ex.Message}"
                };
            }
        }
    }
}
