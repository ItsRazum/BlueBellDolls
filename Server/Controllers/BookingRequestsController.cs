using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BookingRequestsController(IBookingService bookingService, ILogger<BookingRequestsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly ILogger<BookingRequestsController> _logger = logger;

        [EnableRateLimiting("BookingPolicy")]
        [HttpPost]
        public async Task<ActionResult<BookingRequestDetailDto>> CreateBookingRequest(
            [FromBody] CreateBookingRequestDto dto,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(BookingRequestsController), nameof(CreateBookingRequest));
            var result = await _bookingService.AddBookingRequestAsync(dto, token);

            return FromResult(result);
        }
    }
}
