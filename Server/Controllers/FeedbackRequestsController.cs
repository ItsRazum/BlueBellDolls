using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FeedbackRequestsController(
        IFeedbackProcessingService feedbackProcessingService, 
        ILogger<FeedbackRequestsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IFeedbackProcessingService _feedbackProcessingService = feedbackProcessingService;
        private readonly ILogger<FeedbackRequestsController> _logger = logger;

        [EnableRateLimiting("FeedbackRequestPolicy")]
        [HttpPost]
        public async Task<ActionResult<FeedbackRequestDetailDto>> CreateFeedbackRequest(
            [FromBody] CreateFeedbackRequestDto dto,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(FeedbackRequestsController), nameof(CreateFeedbackRequest));
            var result = await _feedbackProcessingService.AddFeedbackRequestAsync(dto, token);
            return FromResult(result);
        }
    }
}
