using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class KittensController(IKittenService kittenService) : BlueBellDollsControllerBase
    {
        private readonly IKittenService _kittenService = kittenService;

        [HttpGet]
        public async Task<ActionResult<PagedResult<KittenListDto>>> GetKittens(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            var result = await _kittenService.GetListAsync(false, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KittenDetailDto>> GetKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            var result = await _kittenService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
