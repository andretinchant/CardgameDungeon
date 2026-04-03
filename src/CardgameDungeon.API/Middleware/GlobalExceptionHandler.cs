using System.Text.Json;
using FluentValidation;

namespace CardgameDungeon.API.Middleware;

public class GlobalExceptionHandler(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, body) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse("Validation failed",
                    validationEx.Errors.Select(e => e.ErrorMessage).ToList())),

            KeyNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ErrorResponse(notFoundEx.Message)),

            InvalidOperationException invalidEx => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(invalidEx.Message)),

            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                new ErrorResponse(argEx.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("An unexpected error occurred."))
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

public record ErrorResponse(string Message, IReadOnlyList<string>? Errors = null);
