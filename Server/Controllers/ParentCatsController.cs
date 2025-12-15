using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ParentCatsController(IParentCatService parentCatService) : BlueBellDollsControllerBase
    {
        private readonly IParentCatService _parentCatService = parentCatService;

        [HttpGet]
        public async Task<ActionResult<PagedResult<ParentCatListDto>>> GetParentCats(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] bool? isMale,
            CancellationToken token = default)
        {
            var result = await _parentCatService.GetListAsync(false, page, pageSize, isMale, token);
            
            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParentCatDetailDto>> GetParentCat(
            [FromRoute] int id, 
            CancellationToken token = default)
        {
            var result = await _parentCatService.GetAsync(false, id, token);
            
            return FromResult(result);
        }
    }
}
