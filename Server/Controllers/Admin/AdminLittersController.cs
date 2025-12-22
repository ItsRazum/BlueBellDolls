using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Interfaces;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/litters")]
    public class AdminLittersController(ILitterService service) : BlueBellDollsControllerBase
    {
        private readonly ILitterService _litterService = service;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<LitterDetailDto>>> GetLitters(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            var result = await _litterService.GetListAsync(true, page, pageSize, token);

            return FromResult(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<LitterDetailDto>> GetLitter(int id, CancellationToken token = default)
        {
            var result = await _litterService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<LitterDetailDto>> CreateLitter([FromBody] CreateLitterDto dto, CancellationToken token = default)
        {
            var result = await _litterService.AddAsync(dto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(GetLitter), new { id = result.Value.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LitterDetailDto>> UpdateLitter(int id, [FromBody] UpdateLitterDto dto, CancellationToken token = default)
        {
            var result = await _litterService.UpdateAsync(id, dto, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLitter(int id, CancellationToken token = default)
        {
            var result = await _litterService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleVisibility(int id, CancellationToken token = default)
        {
            var result = await _litterService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<IActionResult> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            var result = await _litterService.SetDefaultPhotoAsync(id, photoId, token);

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
            var result = await _litterService.UploadFilesAsync(id, dictionaryName, files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<IActionResult> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            var result = await _litterService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }

        [HttpPost("{litterId}/kittens")]
        public async Task<ActionResult<KittenDetailDto>> AddKitten(int litterId, [FromBody] CreateKittenDto dto, CancellationToken token = default)
        {
            var result = await _litterService.AddKittenToLitter(litterId, dto, token);

            return FromResult(result);
        }

        [HttpPut("{litterId}/mother/{parentCatId}")]
        public async Task<ActionResult<LitterDetailDto>> SetMother(int litterId, int parentCatId, CancellationToken token = default)
        {
            var result = await _litterService.SetMotherCatAsync(litterId, parentCatId, token);

            return FromResult(result);
        }

        [HttpPut("{litterId}/father/{parentCatId}")]
        public async Task<ActionResult<LitterDetailDto>> SetFather(int litterId, int parentCatId, CancellationToken token = default)
        {
            var result = await _litterService.SetFatherCatAsync(litterId, parentCatId, token);

            return FromResult(result);
        }
    }
}
