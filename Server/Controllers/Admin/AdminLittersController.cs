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
    [Route("api/admin/litters")]
    public class AdminLittersController(ILitterService service, ILogger<AdminLittersController> logger) : BlueBellDollsControllerBase
    {
        private readonly ILitterService _litterService = service;
        private readonly ILogger<AdminLittersController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<LitterMinimalDto>>> GetLitters(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminLittersController), nameof(GetLitters));
            var result = await _litterService.GetMinimalListAsync(true, page, pageSize, token);

            return FromResult(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<LitterDetailDto>> GetLitter(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(GetLitter), id);
            var result = await _litterService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<LitterDetailDto>> CreateLitter([FromBody] CreateLitterDto dto, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminLittersController), nameof(CreateLitter));
            var result = await _litterService.AddAsync(dto, token);

            if (result.StatusCode != 201 || result.Value is null)
                return FromResult(result);

            return CreatedAtAction(nameof(GetLitter), new { id = result.Value.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LitterDetailDto>> UpdateLitter(int id, [FromBody] UpdateLitterDto dto, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(UpdateLitter), id);
            var result = await _litterService.UpdateAsync(id, dto, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLitter(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(DeleteLitter), id);
            var result = await _litterService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<ActionResult<LitterDetailDto>> ToggleVisibility(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(ToggleVisibility), id);
            var result = await _litterService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<ActionResult<LitterDetailDto>> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(SetDefaultPhoto), id);
            var result = await _litterService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/{dictionaryName:regex(^(photos)$)}")]
        public async Task<ActionResult<EntityFilesUploadResult<LitterDetailDto>>> UploadFiles(
            int id,
            string dictionaryName,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string> telegramFileIds,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(UploadFiles), id);
            var result = await _litterService.UploadFilesAsync(id, dictionaryName, files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<ActionResult<LitterDetailDto>> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminLittersController), nameof(DeleteFiles), id);
            var result = await _litterService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }

        [HttpPost("{litterId}/kittens")]
        public async Task<ActionResult<KittenDetailDto>> AddKitten(int litterId, [FromBody] CreateKittenDto dto, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для litterId = {litterId}", nameof(AdminLittersController), nameof(AddKitten), litterId);
            var result = await _litterService.AddKittenToLitter(litterId, dto, token);

            return FromResult(result);
        }

        [HttpPut("{litterId}/mother/{parentCatId}")]
        public async Task<ActionResult<LitterDetailDto>> SetMother(int litterId, int parentCatId, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminLittersController), nameof(SetMother));
            var result = await _litterService.SetMotherCatAsync(litterId, parentCatId, token);

            return FromResult(result);
        }

        [HttpPut("{litterId}/father/{parentCatId}")]
        public async Task<ActionResult<LitterDetailDto>> SetFather(int litterId, int parentCatId, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminLittersController), nameof(SetFather));
            var result = await _litterService.SetFatherCatAsync(litterId, parentCatId, token);

            return FromResult(result);
        }

        [HttpGet("photos/limit")]
        public ActionResult<PhotosLimitResponse> GetPhotosLimit([FromQuery] PhotosType type)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminLittersController), nameof(GetPhotosLimit));
            var result = _litterService.GetPhotosLimit(type);

            return FromResult(result);
        }
    }
}
