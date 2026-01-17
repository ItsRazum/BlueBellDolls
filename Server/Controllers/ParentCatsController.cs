using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ParentCatsController(IParentCatService parentCatService, ILogger<ParentCatsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IParentCatService _parentCatService = parentCatService;
        private readonly ILogger<ParentCatsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<ParentCatListDto>>> GetParentCats(
            [FromQuery] int page,
            [FromQuery] bool? isMale,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(ParentCatsController), nameof(GetParentCats));
            var result = await _parentCatService.GetListAsync(false, page, 5, isMale, token);
            
            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParentCatDetailDto>> GetParentCat(
            [FromRoute] int id, 
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(ParentCatsController), nameof(GetParentCat), id);
            var result = await _parentCatService.GetAsync(false, id, token);
            
            return FromResult(result);
        }
    }
}
