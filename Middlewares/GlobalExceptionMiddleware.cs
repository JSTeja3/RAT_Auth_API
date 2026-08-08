using System.Net;
using System.Text.Json;
using RAT_AUTH_API.Exceptions;

namespace RAT_AUTH_API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(DuplicateEmailException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch(Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    message = "An unexpected error occurred."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}