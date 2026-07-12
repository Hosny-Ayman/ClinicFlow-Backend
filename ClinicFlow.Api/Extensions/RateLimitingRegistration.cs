using Microsoft.AspNetCore.RateLimiting;

namespace ClinicFlow.Api.Extensions
{
    public static class RateLimitingRegistration
    {

        public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {

                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RateLimiter");
                    logger.LogWarning("Rate limit exceeded. IP: {IP}, Path: {Path}",context.HttpContext.Connection.RemoteIpAddress,context.HttpContext.Request.Path);

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        isSuccess = false,
                        status = "TooManyRequests",
                        errors = new[]
                        {
                            new
                            {
                                code = "TooManyRequests",
                                message = "Too many requests. Please try again later."
                            }
                        }
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                };

                options.AddFixedWindowLimiter("LoginPolicy", limiter =>
                {
                    limiter.Window = TimeSpan.FromMinutes(1);
                    limiter.PermitLimit = 5;
                    limiter.QueueLimit = 0;
                });
            });

            return services;
        }
    }
}
