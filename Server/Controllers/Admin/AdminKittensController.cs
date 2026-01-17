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
    [Route("api/admin/kittens")]
    public class AdminKittensController(IKittenService kittenService, ILogger<AdminKittensController> logger) : BlueBellDollsControllerBase
    {
        private readonly IKittenService _kittenService = kittenService;
        private readonly ILogger<AdminKittensController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<PagedResult<KittenListDto>>> GetKittens(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminKittensController), nameof(GetKittens));
            var result = await _kittenService.GetListAsync(true, page, pageSize, token);

            return FromResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KittenDetailDto>> GetKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(GetKitten), id);
            var result = await _kittenService.GetAsync(true, id, token);

            return FromResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKitten(
            [FromRoute] int id,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(DeleteKitten), id);
            var result = await _kittenService.DeleteAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<KittenDetailDto>> UpdateKitten(
            [FromRoute] int id,
            [FromBody] UpdateKittenDto kittenDto,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(UpdateKitten), id);
            var result = await _kittenService.UpdateAsync(id, kittenDto, token);

            return FromResult(result);
        }

        [HttpPost("{id}/toggle-visibility")]
        public async Task<ActionResult<KittenDetailDto>> ToggleVisibility(int id, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(ToggleVisibility), id);
            var result = await _kittenService.ToggleVisibilityAsync(id, token);

            return FromResult(result);
        }

        [HttpPut("{id}/color")]
        public async Task<ActionResult<KittenDetailDto>> UpdateColor(int id, [FromBody] UpdateColorRequest updateColorRequest, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(UpdateColor), id);
            var result = await _kittenService.UpdateColorAsync(id, updateColorRequest.Color, token);

            return FromResult(result);
        }

        [HttpPost("{id}/class")]
        public async Task<ActionResult<KittenDetailDto>> UpdateClass(
            int id, 
            [FromBody] UpdateKittenClassRequest updateKittenClassRequest, 
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(UpdateClass), id);
            var result = await _kittenService.UpdateKittenClassAsync(id, updateKittenClassRequest, token);

            return FromResult(result);
        }

        [HttpPost("{id}/status")]
        public async Task<ActionResult<KittenDetailDto>> UpdateStatus(
            int id,
            [FromBody] UpdateKittenStatusRequest updateKittenStatusRequest,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(UpdateStatus), id);
            var result = await _kittenService.UpdateKittenStatusAsync(id, updateKittenStatusRequest, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/set-default")]
        public async Task<ActionResult<KittenDetailDto>> SetDefaultPhoto(int id, [FromQuery] int photoId, CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(SetDefaultPhoto), id);
            var result = await _kittenService.SetDefaultPhotoAsync(id, photoId, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos")]
        public async Task<ActionResult<EntityFilesUploadResult<KittenDetailDto>>> UploadFiles(
            int id,
            [FromForm] List<IFormFile> files,
            [FromForm] List<string>? telegramFileIds = null,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(UploadFiles), id);
            var result = await _kittenService.UploadFilesAsync(id, "photos", files, telegramFileIds, token);

            return FromResult(result);
        }

        [HttpPost("{id}/photos/delete-batch")]
        public async Task<ActionResult<KittenDetailDto>> DeleteFiles(
            int id,
            [FromBody] IEnumerable<int> photoIds,
            CancellationToken token = default)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса для id = {id}", nameof(AdminKittensController), nameof(DeleteFiles), id);
            var result = await _kittenService.DeleteFilesAsync(id, photoIds, token);

            return FromResult(result);
        }

        [HttpGet("photos/limit")]
        public ActionResult<PhotosLimitResponse> GetPhotosLimit([FromQuery] PhotosType type)
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminKittensController), nameof(GetPhotosLimit));
            var result = _kittenService.GetPhotosLimit(type);

            return FromResult(result);
        }
    }
}
