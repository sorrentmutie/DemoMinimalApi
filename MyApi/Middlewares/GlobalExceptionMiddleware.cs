using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MyApi.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError("Eccezione non gestita");
                await HandleException(context, ex);
            }
        }


        private static Task HandleException(HttpContext context, Exception ex)
        {
            var (statusCode, title) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
                InvalidProgramException => (StatusCodes.Status406NotAcceptable, "Invalid Operation"),
                ValidationException => (StatusCodes.Status400BadRequest, "Bad Request"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };

            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;


            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Status = statusCode,
                Title = title,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["TraceId"] = traceId;

            return  context.Response.WriteAsJsonAsync( problemDetails);
        }
    }
}
