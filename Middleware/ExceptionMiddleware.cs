using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using HotelListingAPI.VSCode.Exceptions;

namespace HotelListingAPI.VSCode.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var errorDetail = new ErrorDetails
            {
                ErrorType = "Failure",
                ErrorMessage = ex.Message
            };

            // Update status code and error details based on the exception type
            switch (ex)
            {
                case NotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    errorDetail.ErrorType = "Not Found";
                    break;
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorDetail.ErrorType = "Unauthorized";
                    break;
                // Add other specific exceptions if needed
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorDetail.ErrorType = "Internal Server Error";
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            
            var errorDetailJson = JsonSerializer.Serialize(errorDetail, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(errorDetailJson);
        }
    }

    public class ErrorDetails
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
