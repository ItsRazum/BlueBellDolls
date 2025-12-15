using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/kittens")]
    public class AdminKittensController(IKittenService kittenService) : BlueBellDollsControllerBase
    {
        private readonly IKittenService _kittenService = kittenService;

        [HttpGet]
        public async Task<ActionResult<PagedResult<KittenListDto>>> GetKittens(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            var result = await _kittenService.GetListAsync(true, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KittenDetailDto>> GetKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            var result = await _kittenService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            var result = await _kittenService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKitten(
            [FromRoute] int id,
            [FromBody] UpdateKittenDto kittenDto,
            CancellationToken token = default)
        {
            var result = await _kittenService.UpdateAsync(id, kittenDto, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleVisibility(int id, CancellationToken token = default)
        {
            var result = await _kittenService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}/color")]
        public async Task<IActionResult> UpdateColor(int id, [FromBody] UpdateColorRequest updateColorRequest, CancellationToken token = default)
        {
            var result = await _kittenService.UpdateColorAsync(id, updateColorRequest.Color, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<IActionResult> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            var result = await _kittenService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos")]
        public async Task<ActionResult<FileUploadResult[]>> UploadFiles(
            int id,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string>? telegramFileIds = null,
            CancellationToken token = default)
        {
            var result = await _kittenService.UploadFilesAsync(id, "photos", files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<IActionResult> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            var result = await _kittenService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }
    }
}
