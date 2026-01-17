using BlueBellDolls.Common.Enums;
using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Services;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/admin/parentcats")]
    public class AdminParentCatsController(IParentCatService parentCatService, ILogger<AdminParentCatsController> logger) : BlueBellDollsControllerBase
    {
        private readonly IParentCatService _parentCatService = parentCatService;
        private readonly ILogger<AdminParentCatsController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<ParentCatListDto>>> GetParentCats(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] bool? isMale,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminParentCatsController), nameof(GetParentCats));
            var result = await _parentCatService.GetListAsync(true, page, pageSize, isMale, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParentCatDetailDto>> GetParentCat(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(GetParentCat), id);
            var result = await _parentCatService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParentCat(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(DeleteParentCat), id);
            var result = await _parentCatService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ParentCatDetailDto>> AddParentCat(
            [FromBody] CreateParentCatDto parentCatDto,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminParentCatsController), nameof(AddParentCat));
            var result = await _parentCatService.AddAsync(parentCatDto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(GetParentCat), new { id = result.Value.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ParentCatDetailDto>> UpdateParentCat(
            [FromRoute] int id,
            [FromBody] UpdateParentCatDto parentCatDto,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(UpdateParentCat), id);
            var result = await _parentCatService.UpdateAsync(id, parentCatDto, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<ActionResult<ParentCatDetailDto>> ToggleVisibility(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(ToggleVisibility), id);
            var result = await _parentCatService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}/color")]
        public async Task<ActionResult<ParentCatDetailDto>> UpdateColor(int id, [FromBody] UpdateColorRequest updateColorRequest, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(UpdateColor), id);
            var result = await _parentCatService.UpdateColorAsync(id, updateColorRequest.Color, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<ActionResult<ParentCatDetailDto>> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(SetDefaultPhoto), id);
            var result = await _parentCatService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/{dictionaryName:regex(^(photos|titles|gentests)|)}")]
        public async Task<ActionResult<EntityFilesUploadResult<ParentCatDetailDto>>> UploadFiles(
            int id,
            string dictionaryName,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string>? telegramFileIds = null,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(UploadFiles), id);
            var result = await _parentCatService.UploadFilesAsync(id, dictionaryName, files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<ActionResult<ParentCatDetailDto>> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminParentCatsController), nameof(DeleteFiles), id);
            var result = await _parentCatService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }

        [HttpGet("photos/limit")]
        public ActionResult<PhotosLimitResponse> GetPhotosLimit([FromQuery] PhotosType type)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminParentCatsController), nameof(GetPhotosLimit));
            var result = _parentCatService.GetPhotosLimit(type);

            return FromResult(result);
        }
    }
}
