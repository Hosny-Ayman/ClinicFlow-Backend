using ClinicFlow.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Extensions
{
    public static class ControllerResponseExtensions
    {


        public static IActionResult ToHttpResponse<T>(this ControllerBase controller, OperationResult<T> result)
        {
            return result.Status switch
            {
                OperationStatus.Success => controller.Ok(result),
                OperationStatus.Conflict => controller.Conflict(result),
                OperationStatus.NotFound => controller.NotFound(result),
                OperationStatus.BadRequest => controller.BadRequest(result),
                OperationStatus.Unauthorized => controller.Unauthorized(result),
                _ => controller.StatusCode(500, result)

            };
        }


    }
}
