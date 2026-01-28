using BlueBellDolls.Common.Records.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Types
{
    public abstract class BlueBellDollsControllerBase : ControllerBase
    {
        protected ActionResult<T> FromCode<T>(int code, T? value) where T : class
        {
            return GetResult(code, null, value);
        }

        protected IActionResult FromResult(ServiceResult result)
        {
            return GetResult<object>(result.StatusCode);
        }

        protected ActionResult<T> FromResult<T>(ServiceResult<T> result) where T : class
        {
            return GetResult(result.StatusCode, result.Message, result.Value);
        }

        private ActionResult GetResult<T>(int code, string? message = null, T? value = null) where T : class
        {
            return code switch
            {
                StatusCodes.Status200OK => value == null ? Ok() : Ok(value),
                _ => StatusCode(code, new { message }),
            };
        }
    }
}
