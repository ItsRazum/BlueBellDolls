using BlueBellDolls.Common.Extensions;
using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Data.Interfaces;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Services
{
    public class FeedbackProcessingService(
        IApplicationDbContext applicationDbContext,
        IBotService botService,
        ICommonMessageParametersProvider commonMessageParametersProvider,
        IOptions<TelegramNotificationSettings> options,
        ILogger<FeedbackProcessingService> logger) : IFeedbackProcessingService
    {

        #region Fields

        private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IBotService _botService = botService;
        private readonly ICommonMessageParametersProvider _commonMessageParametersProvider = commonMessageParametersProvider;
        private readonly TelegramNotificationSettings _telegramNotificationSettings = options.Value;
        private readonly ILogger<FeedbackProcessingService> _logger = logger;

        #endregion

        #region Methods

        public async Task<ServiceResult<FeedbackRequestDetailDto>> AddFeedbackRequestAsync(CreateFeedbackRequestDto feedbackRequest, CancellationToken token)
        {
            try
            {
                var dateTime = DateTime.UtcNow.AddDays(1);
                if (_applicationDbContext.FeedbackRequests.Any(fr => fr.Phone == feedbackRequest.PhoneNumber && !fr.IsProcessed && fr.CreatedAt < dateTime))
                    return new(StatusCodes.Status403Forbidden, "Запрос на обратную связь с таким номером телефона уже существует и ожидает обработки!");

                var previousRequest = await _applicationDbContext.FeedbackRequests
                    .Where(fr => fr.Phone == feedbackRequest.PhoneNumber && fr.IsProcessed)
                    .OrderByDescending(fr => fr.CreatedAt)
                    .FirstOrDefaultAsync(token);

                var entity = new FeedbackRequest
                {
                    Name = feedbackRequest.Name,
                    Phone = feedbackRequest.PhoneNumber,
                    IsProcessed = false
                };
                _applicationDbContext.FeedbackRequests.Add(entity);
                await _applicationDbContext.SaveChangesAsync(token);

                var messageParameters = _commonMessageParametersProvider.GetNewFeedbackRequestParameters(entity, previousRequest);
                await _botService.SendMessageAsync(_telegramNotificationSettings.NotificationsGroupId, messageParameters, token);
                return new ServiceResult<FeedbackRequestDetailDto>(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось отправить уведомление о новом запросе на обратную связь!");
                return new(StatusCodes.Status500InternalServerError, "Не удалось отправить уведомление о новом запросе на обратную связь!");
            }
        }

        public async Task<ServiceResult<FeedbackRequestDetailDto>> MarkFeedbackRequestAsProcessedAsync(int feedbackRequestId, CancellationToken token)
        {
            try
            {
                var entity = await _applicationDbContext.FeedbackRequests.FindAsync([feedbackRequestId], token);
                if (entity == null)
                    return new(StatusCodes.Status404NotFound, "Запрос на обратную связь не найден!");

                entity.IsProcessed = true;
                await _applicationDbContext.SaveChangesAsync(token);
                return new(StatusCodes.Status200OK, null, entity.ToDetailDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось закрыть запрос на обратную связь!");
                return new(StatusCodes.Status500InternalServerError, $"Не удалось закрыть запрос на обратную связь: {ex.Message}");

            }

        }

        #endregion

    }
}
