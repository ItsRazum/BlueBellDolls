using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LittersController(ILitterService service) : BlueBellDollsControllerBase
    {
        private readonly ILitterService _litterService = service;

        [HttpGet]
        public async Task<ActionResult<PagedResult<LitterDetailDto>>> GetLitters(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            var result = await _litterService.GetListAsync(false, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LitterDetailDto>> GetLitter(int id, CancellationToken token = default)
        {
            var result = await _litterService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
