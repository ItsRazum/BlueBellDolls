using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatColorsController(ICatColorService catColorService) : BlueBellDollsControllerBase
    {
        private readonly ICatColorService _catColorService = catColorService;

        [HttpGet]
        public async Task<ActionResult<PagedResult<CatColorListDto>>> GetCatColors(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            var result = await _catColorService.GetListAsync(false, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatColorDetailDto>> GetCatColor(int id, CancellationToken token = default)
        {
            var result = await _catColorService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
