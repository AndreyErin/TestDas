using Npgsql;
using System.Net;
using TestDas.Models;

namespace TestDas.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NpgsqlException ex)
            {
                await HandleException(httpContext, ex, HttpStatusCode.InternalServerError);
            }
            // и тд...................
            catch (Exception ex)
            {
                await HandleException(httpContext, ex, HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleException(
            HttpContext httpContext,
            Exception exception,
            HttpStatusCode httpStatusCode)
        {

            var response = new HtmlExtractionResponse
            {
                IsError = 1,
                ErrorCode = exception.GetType().Name,
                ErrorMessage = exception.Message
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)httpStatusCode;

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}