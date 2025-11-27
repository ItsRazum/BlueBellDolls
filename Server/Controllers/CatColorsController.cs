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
            var result = await _catColorService.GetListAsync(page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatColorDetailDto>> GetCatColor(int id, CancellationToken token = default)
        {
            var result = await _catColorService.GetAsync(id, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<CatColorDetailDto>> CreateCatColor([FromBody] CreateCatColorDto dto, CancellationToken token = default)
        {
            var result = await _catColorService.AddAsync(dto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(GetCatColor), new { id = result.Value.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatColor([FromRoute] int id, CancellationToken token = default)
        {
            var result = await _catColorService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleVisibility(int id, CancellationToken token = default)
        {
            var result = await _catColorService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<IActionResult> SetDefaultPhoto(
            [FromRoute] int id,
            [FromQuery] int photoId,
            CancellationToken token = default)
        {
            var result = await _catColorService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/{dictionaryName:regex(^(photos)$)}")]
        public async Task<ActionResult<FileUploadResult[]>> UploadFiles(
            int id,
            string dictionaryName,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string> telegramFileIds,
            CancellationToken token = default)
        {
            var result = await _catColorService.UploadFilesAsync(id, dictionaryName, files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<IActionResult> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            var result = await _catColorService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }
    }
}
