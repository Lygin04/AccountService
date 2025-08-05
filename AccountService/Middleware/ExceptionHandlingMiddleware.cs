using System.Text.Json;
using AccountService.Common;
using AccountService.Exceptions;
using AccountService.Exceptions.Shared;

namespace AccountService.Middleware;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception occurred: {Message}", e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var (statusCode, mbError) = GetMbError(exception);

        var response = MbResult<object>.Failure(mbError);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            response.IsSuccess,
            response.Error!.Title,
            response.Error!.Status,
            response.Error!.Detail,
            response.Error!.Errors
        }));
    }

    private static (int StatusCode, MbError MbError) GetMbError(Exception exception)
    {
        return exception switch
        {
            ValidationAppException validationException => (
                StatusCodes.Status422UnprocessableEntity,
                new MbError(
                    title: "Validation Error",
                    status: StatusCodes.Status422UnprocessableEntity,
                    detail: validationException.Message,
                    errors: validationException.Errors
                )
            ),
            BadRequestException badRequest => (
                StatusCodes.Status400BadRequest,
                new MbError(
                    title: "Bad Request",
                    status: StatusCodes.Status400BadRequest,
                    detail: badRequest.Message
                )
            ),
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                new MbError(
                    title: "Not Found",
                    status: StatusCodes.Status404NotFound,
                    detail: notFound.Message
                )
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                new MbError(
                    title: "Server Error",
                    status: StatusCodes.Status500InternalServerError,
                    detail: exception.Message
                )
            )
        };
    }
}