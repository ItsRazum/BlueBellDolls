using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/admin/parentcats")]
    public class AdminParentCatsController(IParentCatService parentCatService) : BlueBellDollsControllerBase
    {

        private readonly IParentCatService _parentCatService = parentCatService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ParentCatListDto>>> GetParentCats(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] bool? isMale,
            CancellationToken token = default)
        {
            var result = await _parentCatService.GetListAsync(true, page, pageSize, isMale, token);

            return FromResult(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ParentCatDetailDto>> GetParentCat(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            var result = await _parentCatService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParentCat(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            var result = await _parentCatService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ParentCatDetailDto>> AddParentCat(
            [FromBody] CreateParentCatDto parentCatDto,
            CancellationToken token = default)
        {
            var result = await _parentCatService.AddAsync(parentCatDto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(GetParentCat), new { id = result.Value.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParentCat(
            [FromRoute] int id,
            [FromBody] UpdateParentCatDto parentCatDto,
            CancellationToken token = default)
        {
            var result = await _parentCatService.UpdateAsync(id, parentCatDto, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleVisibility(int id, CancellationToken token = default)
        {
            var result = await _parentCatService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}/color")]
        public async Task<IActionResult> UpdateColor(int id, [FromBody] UpdateColorRequest updateColorRequest, CancellationToken token = default)
        {
            var result = await _parentCatService.UpdateColorAsync(id, updateColorRequest.Color, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<IActionResult> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            var result = await _parentCatService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/{dictionaryName:regex(^(photos|titles|gentests)|)}")]
        public async Task<ActionResult<FileUploadResult[]>> UploadFiles(
            int id,
            string dictionaryName,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string>? telegramFileIds = null,
            CancellationToken token = default)
        {
            var result = await _parentCatService.UploadFilesAsync(id, dictionaryName, files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<IActionResult> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            var result = await _parentCatService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }
    }
}
