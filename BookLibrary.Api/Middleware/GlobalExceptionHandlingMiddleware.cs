using System.Net;
using BookLibrary.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookLibrary.Api.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException ex)
            {
                await WriteProblemDetails(context, ex.Message, ex.Errors, StatusCodes.Status400BadRequest, "https://httpstatuses.com/400");
            }
            catch (InvalidOperationException ex)
            {
                await WriteProblemDetails(context, ex.Message, null, StatusCodes.Status400BadRequest, "https://httpstatuses.com/400");
            }
            catch (Exception ex)
            {
                await WriteProblemDetails(context, "An unexpected error occurred.", new[] { ex.Message }, StatusCodes.Status500InternalServerError, "https://httpstatuses.com/500");
            }
        }

        private static async Task WriteProblemDetails(HttpContext context, string detail, IEnumerable<string>? errors, int statusCode, string type)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;
            var pd = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode == 500 ? "Server Error" : "Request Error",
                Detail = detail,
                Type = type
            };
            if (errors != null)
            {
                pd.Extensions["errors"] = errors;
            }
            var json = JsonSerializer.Serialize(pd);
            await context.Response.WriteAsync(json);
        }
    }
}
