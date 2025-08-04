using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Common;

[Authorize]
[ApiController]
[Route("v1/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ApiControllerV1WithAuth : ControllerBase
{
    protected IActionResult ToActionResult<T>(MbResult<T> result)
    {
        if (result.IsSuccess)
        {
            return typeof(T) == typeof(Unit)
                ? StatusCode(StatusCodes.Status204NoContent)
                : Ok(result.Data);
        }

        return StatusCode(result.Error!.Status, new
        {
            result.Error.Title,
            result.Error.Status,
            result.Error.Detail,
            result.Error.Errors
        });
    }
}