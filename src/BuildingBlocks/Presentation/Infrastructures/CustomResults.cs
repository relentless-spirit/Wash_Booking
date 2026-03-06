using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Http;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace BuildingBlocks.Presentation.Infrastructures;

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException();

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: GetErrors(result));
    }

    public static IResult Problem<TValue>(Result<TValue> result)
        where TValue : notnull
    {
        if (result.IsSuccess)
            throw new InvalidOperationException();

        return Results.Problem(
            title: GetTitle(result.FirstError),
            detail: GetDetail(result.FirstError),
            type: GetType(result.FirstError.Type),
            statusCode: GetStatusCode(result.FirstError.Type),
            extensions: GetErrors(result));
    }

    private static string GetTitle(Error error) =>
        error.Type switch
        {
            ErrorType.Unexpected => "Server failure",
            _ => error.Code
        };

    private static string GetDetail(Error error) =>
        error.Type switch
        {
            ErrorType.Unexpected => "An unexpected error occurred",
            _ => error.Message
        };

    private static string GetType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation  => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Failure     => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Custom      => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ErrorType.Forbidden   => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            ErrorType.NotFound    => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            ErrorType.Conflict    => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ErrorType.Unexpected  => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _                     => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or 
            ErrorType.Failure or 
            ErrorType.Custom      => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden   => StatusCodes.Status403Forbidden,
            ErrorType.NotFound    => StatusCodes.Status404NotFound,
            ErrorType.Conflict    => StatusCodes.Status409Conflict,
            ErrorType.Unexpected  => StatusCodes.Status500InternalServerError,
            _                     => StatusCodes.Status500InternalServerError
        };

    private static Dictionary<string, object?>? GetErrors(Result result)
    {
        if (result.Error is not ValidationError validationError)
            return null;

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }

    private static Dictionary<string, object?>? GetErrors<TValue>(Result<TValue> result)
        where TValue : notnull
    {
        if (result.FirstError is not ValidationError validationError)
            return null;

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }
}