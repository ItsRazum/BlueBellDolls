using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Records;
using Microsoft.EntityFrameworkCore;

namespace BlueBellDolls.Server.Services
{
    public class BookingService(IApplicationDbContext applicationDbContext) : IBookingService
    {

        #region Fields

        private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;

        #endregion

        #region IBookingService implementation

        public async Task<ServiceResult<BookingRequest>> AddBookingRequestAsync(string customerName, string customerPhone, int kittenId, CancellationToken token = default)
        {
            var (valid, statusCode, validationMessage) = await ValidateBookingRequestAsync(customerName, customerPhone, kittenId, token);

            if (!valid)
                return new(statusCode, validationMessage, null);

            var bookingRequest = new BookingRequest
            {
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                KittenId = kittenId,
                IsProcessed = false
            };

            await _applicationDbContext.BookingRequests.AddAsync(bookingRequest, token);
            await _applicationDbContext.SaveChangesAsync(token);

            return new(StatusCodes.Status201Created, "Бронь успешно оформлена!", bookingRequest);
        }

        public async Task<ServiceResult> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var bookingRequest = await _applicationDbContext.BookingRequests.FindAsync([bookingId], token);
            if (bookingRequest == null)
                return new(StatusCodes.Status404NotFound, "Бронь не найдена.");

            if (bookingRequest.CuratorTelegramId == null)
                return new(StatusCodes.Status400BadRequest, "Эта бронь не назначена на куратора!");

            if (telegramUserId != bookingRequest.CuratorTelegramId)
                return new(StatusCodes.Status403Forbidden, "У вас нет прав для закрытия этой брони!");

            bookingRequest.IsProcessed = true;
            await _applicationDbContext.SaveChangesAsync(token);

            return new(StatusCodes.Status200OK);
        }

        public async Task<ServiceResult> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            var bookingRequest = await _applicationDbContext.BookingRequests.FindAsync([bookingId], token);
            if (bookingRequest == null)
                return new(StatusCodes.Status404NotFound, "Бронь не найдена.");

            if (bookingRequest.CuratorTelegramId != null)
                return new(StatusCodes.Status403Forbidden, "Эта бронь уже назначена на куратора!");

            bookingRequest.CuratorTelegramId = telegramUserId;
            await _applicationDbContext.SaveChangesAsync(token);

            return new(StatusCodes.Status200OK);
        }

        #endregion

        #region Private methods

        private async Task<(bool valid, int statusCode, string message)> ValidateBookingRequestAsync(string customerName, string customerPhone, int kittenId, CancellationToken token = default)
        {
            if (customerName is not
                {
                    Length: < 3 or > 30
                })
                return (false, StatusCodes.Status400BadRequest, "Имя введено некорректно.");

            if (customerPhone is not
                {
                    Length: < 3 or > 30
                })
                return (false, StatusCodes.Status400BadRequest, "Телефон введён некорректно.");

            if (kittenId <= 0)
                return (false, StatusCodes.Status400BadRequest, "Некорректный идентификатор котенка.");

            var spamCheckTime = DateTime.UtcNow.AddDays(-1);
            var hasRecentBooking = await _applicationDbContext.BookingRequests
                .AnyAsync(br => br.CustomerPhone == customerPhone && br.KittenId == kittenId && br.CreatedAt > spamCheckTime, token);

            if (hasRecentBooking)
                return (false, StatusCodes.Status429TooManyRequests, "Бронь с таким телефоном уже существует для данного котенка");

            var kitten = await _applicationDbContext.Kittens.FindAsync([kittenId], token);
            if (kitten == null)
                return (false, StatusCodes.Status404NotFound, "Котенок не найден.");

            if (kitten.Status != KittenStatus.Available || !kitten.IsEnabled)
                return (false, StatusCodes.Status403Forbidden, "Этот котенок недоступен для бронирования.");

            return (true, StatusCodes.Status200OK, string.Empty);
        }

        #endregion

    }
}
