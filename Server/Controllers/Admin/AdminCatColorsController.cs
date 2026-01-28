using BlueBellDolls.Common.Dtos;
using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Models;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/catcolors")]
    public class AdminCatColorsController(ICatColorService catColorService, ILogger<AdminCatColorsController> logger) : BlueBellDollsControllerBase
    {
        private readonly ICatColorService _catColorService = catColorService;
        private readonly ILogger<AdminCatColorsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<CatColorListDto>>> GetCatColors(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminCatColorsController), nameof(GetCatColors));
            var result = await _catColorService.GetListAsync(true, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatColorDetailDto>> GetCatColor(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminCatColorsController), nameof(GetCatColor), id);
            var result = await _catColorService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpGet("identifier/{identifier}")]
        public async Task<ActionResult<CatColorDetailDto>> GetCatColor(
            string identifier, 
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для identifier = {identifier}", nameof(AdminCatColorsController), nameof(GetCatColor), identifier);
            var result = await _catColorService.GetAsync(true, identifier, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<CatColorDetailDto>> CreateCatColor([FromBody] CreateCatColorDto dto, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminCatColorsController), nameof(CreateCatColor));
            var result = await _catColorService.AddAsync(dto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(CatColor), new { id = result.Value.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatColor([FromRoute] int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminCatColorsController), nameof(DeleteCatColor), id);
            var result = await _catColorService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<ActionResult<CatColorDetailDto>> ToggleVisibility(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminCatColorsController), nameof(ToggleVisibility), id);
            var result = await _catColorService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CatColorDetailDto>> UpdateCatColor(
            int id,
            [FromBody] UpdateCatColorDto dto,
            CancellationToken token = default)
        {
            var result = await _catColorService.UpdateAsync(id, dto, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<ActionResult<CatColorDetailDto>> SetDefaultPhoto(
            [FromRoute] int id,
            [FromQuery] int photoId,
            CancellationToken token = default)
        {
            var result = await _catColorService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/{dictionaryName:regex(^(photos)$)}")]
        public async Task<ActionResult<EntityFilesUploadResult<CatColorDetailDto>>> UploadFiles(
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
        public async Task<ActionResult<CatColorDetailDto>> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            var result = await _catColorService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }

        [HttpGet("tree")]
        public async Task<ActionResult<CatColorTree>> GetColorTree(CancellationToken token = default)
        {
            var result = await _catColorService.GetColorTreeAsync(token);
            return FromResult(result);
        }

        [HttpGet("photos/limit")]
        public ActionResult<PhotosLimitResponse> GetPhotosLimit([FromQuery] PhotosType type)
        {
            var result = _catColorService.GetPhotosLimit(type);

            return FromResult(result);
        }
    }
}
