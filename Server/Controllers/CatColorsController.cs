using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatColorsController(ICatColorService catColorService, ILogger<CatColorsController> logger) : BlueBellDollsControllerBase
    {
        private readonly ICatColorService _catColorService = catColorService;
        private readonly ILogger<CatColorsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<CatColorListDto>>> GetCatColors(
            [FromQuery] int page,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(CatColorsController), nameof(GetCatColors));
            var result = await _catColorService.GetListAsync(false, page, 5, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatColorDetailDto>> GetCatColor(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(CatColorsController), nameof(GetCatColor), id);
            var result = await _catColorService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
