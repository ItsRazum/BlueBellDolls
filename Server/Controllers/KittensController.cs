using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class KittensController(IKittenService kittenService, ILogger<KittensController> logger) : BlueBellDollsControllerBase
    {
        private readonly IKittenService _kittenService = kittenService;
        private readonly ILogger<KittensController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<KittenListDto[]>> GetKittens(CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(KittensController), nameof(GetKittens));
            var result = await _kittenService.GetFreeKittensAsync(token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KittenDetailDto>> GetKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(KittensController), nameof(GetKitten), id);
            var result = await _kittenService.GetAsync(false, id, token);

            return FromResult(result);
        }
    }
}
