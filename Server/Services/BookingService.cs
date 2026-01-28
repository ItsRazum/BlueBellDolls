using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Services
{
    public class BookingService(
        IApplicationDbContext applicationDbContext, 
        IBotService botService,
        ICommonMessageParametersProvider commonMessageParametersProvider,
        IOptions<TelegramNotificationSettings> options,
        ILogger<BookingService> logger) : IBookingService
    {

        #region Fields

        private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IBotService _botService = botService;
        private readonly ICommonMessageParametersProvider _commonMessageParametersProvider = commonMessageParametersProvider;
        private readonly TelegramNotificationSettings _telegramNotificationSettings = options.Value;
        private readonly ILogger<BookingService> _logger = logger;

        #endregion

        #region IBookingService implementation

        public async Task<ServiceResult<BookingRequestDetailDto>> AddBookingRequestAsync(CreateBookingRequestDto dto, CancellationToken token = default)
        {
            try
            {
                var (valid, statusCode, validationMessage) = await ValidateBookingRequestAsync(dto, token);

                if (!valid)
                {
                    _logger.LogWarning("Запрос на бронь не прошёл валидацию: {message}\n" +
                        "Имя: {name}\n" +
                        "Номер телефона: {phoneNumber}\n" +
                        "KittenId: {kittenId}", 
                        validationMessage,
                        dto.Name,
                        dto.PhoneNumber,
                        dto.KittenId);
                    return new(statusCode, validationMessage, null);
                }

                var bookingRequest = dto.ToEFModel();

                await _applicationDbContext.BookingRequests.AddAsync(bookingRequest, token);
                await _applicationDbContext.SaveChangesAsync(token);

                await _botService.SendMessageAsync(
                    _telegramNotificationSettings.NotificationsGroupId,
                    _commonMessageParametersProvider.GetNewBookingRequestParameters(bookingRequest),
                    token: token);

                return new(StatusCodes.Status201Created, "Бронь успешно оформлена!", bookingRequest.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(BookingService), nameof(AddBookingRequestAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось добавить бронь!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось оформить бронь");
            }
        }

        public async Task<ServiceResult<BookingRequestDetailDto>> CloseBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var bookingRequest = await _applicationDbContext.BookingRequests.FindAsync([bookingId], token);
                if (bookingRequest == null)
                    return new(StatusCodes.Status404NotFound, "Бронь не найдена.");

                if (bookingRequest.CuratorTelegramId == 0)
                    return new(StatusCodes.Status400BadRequest, "Эта бронь не назначена на куратора!");

                if (telegramUserId != bookingRequest.CuratorTelegramId)
                    return new(StatusCodes.Status403Forbidden, "У вас нет прав для закрытия этой брони!");

                bookingRequest.IsProcessed = true;
                await _applicationDbContext.SaveChangesAsync(token);

                return new(StatusCodes.Status200OK, Value: bookingRequest.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(BookingService), nameof(CloseBookingRequestAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось закрыть бронь {id}!", bookingId);
                return new(StatusCodes.Status500InternalServerError, "Не удалось закрыть бронь");
            }
        }

        public async Task<ServiceResult<BookingRequestDetailDto>> ProcessBookingRequestAsync(int bookingId, long telegramUserId, CancellationToken token = default)
        {
            try
            {
                var bookingRequest = await _applicationDbContext.BookingRequests.FindAsync([bookingId], token);
                if (bookingRequest == null)
                    return new(StatusCodes.Status404NotFound, "Бронь не найдена.");

                if (bookingRequest.CuratorTelegramId != 0)
                    return new(StatusCodes.Status403Forbidden, "Эта бронь уже назначена на куратора!");

                bookingRequest.CuratorTelegramId = telegramUserId;
                await _applicationDbContext.SaveChangesAsync(token);

                return new(StatusCodes.Status200OK, Value: bookingRequest.ToDetailDto());
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("{service}.{method}(): Операция была отменена", nameof(BookingService), nameof(ProcessBookingRequestAsync));
                return new(StatusCodes.Status403Forbidden, "Операция отменена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обработать бронь {id}!", bookingId);
                return new(StatusCodes.Status500InternalServerError, "Не удалось обработать бронь");
            }
        }

        #endregion

        #region Private methods

        private async Task<(bool valid, int statusCode, string message)> ValidateBookingRequestAsync(CreateBookingRequestDto dto, CancellationToken token = default)
        {
            if (dto.Name is
                {
                    Length: < 3 or > 30
                })
                return (false, StatusCodes.Status400BadRequest, "Имя введено некорректно.");

            if (dto.PhoneNumber is
                {
                    Length: < 3 or > 20
                })
                return (false, StatusCodes.Status400BadRequest, "Телефон введён некорректно.");

            if (dto.KittenId <= 0)
                return (false, StatusCodes.Status400BadRequest, "Некорректный идентификатор котенка.");

            var spamCheckTime = DateTime.UtcNow.AddDays(1);
            var hasRecentBooking = await _applicationDbContext.BookingRequests
                .CountAsync(br => br.CustomerPhone == dto.PhoneNumber && br.KittenId == dto.KittenId && br.CreatedAt < spamCheckTime, token) != 0;

            if (hasRecentBooking)
                return (false, StatusCodes.Status429TooManyRequests, "Заявка на бронь с таким телефоном уже существует для данного котенка. Пожалуйста, подождите немного.");

            var kitten = await _applicationDbContext.Kittens.FindAsync([dto.KittenId], token);
            if (kitten == null)
                return (false, StatusCodes.Status404NotFound, "К сожалению, этот котёнок не найден в базе.");

            if (kitten.Status != KittenStatus.Available || !kitten.IsEnabled)
                return (false, StatusCodes.Status403Forbidden, "К сожалению, сейчас этот котёнок недоступен для бронирования.");

            return (true, StatusCodes.Status200OK, string.Empty);
        }

        #endregion

    }
}
