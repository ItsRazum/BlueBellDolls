using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BookingRequestsController(IBookingService bookingService) : BlueBellDollsControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;

        [EnableRateLimiting("BookingPolicy")]
        [HttpPost]
        public async Task<ActionResult<BookingRequest>> CreateBookingRequest(
            [FromBody] CreateBookingRequestDto dto,
            CancellationToken token = default)
        {
            var result = await _bookingService.AddBookingRequestAsync(dto.CustomerName, dto.CustomerPhone, dto.KittenId, token);

            return FromResult(result);
        }
    }
}
