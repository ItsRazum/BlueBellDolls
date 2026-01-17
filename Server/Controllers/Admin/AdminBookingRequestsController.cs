using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/admin/bookingrequests")]
    public class AdminBookingRequestsController(IBookingService bookingService, ILogger<AdminBookingRequestsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly ILogger<AdminBookingRequestsController> _logger = logger;

        [HttpPost("{id}/process")]
        public async Task<ActionResult<BookingRequestDetailDto>> ProcessBookingRequest(
            int id,
            [FromQuery] long telegramUserId,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminBookingRequestsController), nameof(ProcessBookingRequest));
            var result = await _bookingService.ProcessBookingRequestAsync(id, telegramUserId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/close")]
        public async Task<ActionResult<BookingRequestDetailDto>> CloseBookingRequest(
            int id,
            [FromQuery] long telegramUserId,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminBookingRequestsController), nameof(CloseBookingRequest));
            var result = await _bookingService.CloseBookingRequestAsync(id, telegramUserId, token);

            return FromResult(result);
        }
    }
}
