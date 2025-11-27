using BlueBellDolls.Common.Interfaces;
using BlueBellDolls.Common.Types;
using BlueBellDolls.Server.Records;
using Microsoft.AspNetCore.Mvc;

namespace BlueBellDolls.Server.Types
{
    public abstract class BlueBellDollsControllerBase : ControllerBase
    {
        protected ActionResult<T> FromCode<T>(int code, T? value) where T : class
        {
            return GetResult(code, value);
        }

        protected IActionResult FromResult(ServiceResult result)
        {
            return GetResult<object>(result.StatusCode);
        }

        protected ActionResult<T> FromResult<T>(ServiceResult<T> result) where T : class
        {
            return GetResult(result.StatusCode, result.Value);
        }

        private ActionResult GetResult<T>(int code, T? value = null) where T : class
        {
            return code switch
            {
                StatusCodes.Status200OK => value == null ? Ok() : Ok(value),
                StatusCodes.Status204NoContent => NoContent(),
                StatusCodes.Status400BadRequest => BadRequest(),
                StatusCodes.Status404NotFound => NotFound(),
                StatusCodes.Status500InternalServerError => StatusCode(500, "error"),
                _ => StatusCode(code),
            };
        }
    }
}
