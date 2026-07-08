using ClinicFlow.Application.Common.Errors;
using ClinicFlow.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Extensions;

public static class ApiBehaviorRegistration
{
    public static IServiceCollection AddApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => new Error("ValidationError", x.ErrorMessage))
                    .ToList();

                var result = OperationResult<bool>.BadRequest(errors);

                return new BadRequestObjectResult(result);
            };
        });

        return services;
    }
}