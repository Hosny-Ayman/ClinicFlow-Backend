using ClinicFlow.Api.Middlewares;

namespace ClinicFlow.Api.Extensions
{
    public static class MiddlewareRegistration
    {

        public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            return app;
        }


    }
}
