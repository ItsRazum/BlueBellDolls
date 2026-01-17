using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LittersController(ILitterService service, ILogger<LittersController> logger) : BlueBellDollsControllerBase
    {
        private readonly ILitterService _litterService = service;
        private readonly ILogger<LittersController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<LitterDetailDto>>> GetLitters(
            [FromQuery] int page,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(LittersController), nameof(GetLitters));
            var result = await _litterService.GetDetailListAsync(false, page, 3, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LitterDetailDto>> GetLitter(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(LittersController), nameof(GetLitter), id);
            var result = await _litterService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
