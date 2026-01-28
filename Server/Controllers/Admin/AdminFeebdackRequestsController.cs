using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [ApiController]
    [Route("/api/admin/feedbackrequests")]
    public class AdminFeebdackRequestsController(
        IFeedbackProcessingService feedbackProcessingService,
        ILogger<FeedbackRequestsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IFeedbackProcessingService _feedbackProcessingService = feedbackProcessingService;
        private readonly ILogger<FeedbackRequestsController> _logger = logger;

        [HttpPost("{id}/close")]
        public async Task<ActionResult<FeedbackRequestDetailDto>> CloseFeedbackRequest(
            int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(FeedbackRequestsController), nameof(CloseFeedbackRequest));
            var result = await _feedbackProcessingService.MarkFeedbackRequestAsProcessedAsync(id, token);
            return FromResult(result);
        }
    }
}
