using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/admin/bookingrequests")]
    public class AdminBookingRequestsController(IBookingService bookingService) : BlueBellDollsControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessBookingRequest(
            int id,
            [FromQuery] long telegramUserId,
            CancellationToken token = default)
        {
            var result = await _bookingService.ProcessBookingRequestAsync(id, telegramUserId, token);
            return FromResult(result);
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseBookingRequest(
            int id,
            [FromQuery] long telegramUserId,
            CancellationToken token = default)
        {
            var result = await _bookingService.CloseBookingRequestAsync(id, telegramUserId, token);
            return FromResult(result);
        }
    }
}
