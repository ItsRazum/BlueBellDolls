using BlueBellDolls.Common.Records.Dtos;
using BlueBellDolls.Server.Settings;
using BlueBellDolls.Server.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlueBellDolls.Server.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("admin/configuration")]
    public class AdminConfigurationController(IOptions<FileStorageSettings> fileStorageSettings, ILogger<AdminConfigurationController> logger) : BlueBellDollsControllerBase
    {
        private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;
        private readonly ILogger<AdminConfigurationController> _logger = logger;

        [HttpGet("photos/limit")]
        public ActionResult<PhotosLimitsResponse> GetPhotosLimit()
        {
            _logger.LogInformation("{controller}.{method}(): Идёт обработка запроса", nameof(AdminConfigurationController), nameof(GetPhotosLimit));
            return Ok(new PhotosLimitsResponse(_fileStorageSettings.Limits));
        }
    }
}
