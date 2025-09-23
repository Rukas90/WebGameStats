using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Results;

public static class Failure
{
    public static ProblemDetails Error(IdentityResult result)
        => new()
        {
            Detail = result.Errors.Select(error => error.Description).FirstOrDefault(),
            Status = StatusCodes.Status400BadRequest
        };
    
    public static ProblemDetails Error(IdentityError error) 
        => new()
        {
            Detail = error.Description,
            Status = StatusCodes.Status400BadRequest
        };
    
    public static ValidationProblemDetails Error(IEnumerable<IdentityError> errors)
    {
        var errorDict = new Dictionary<string, string[]>
        {
            ["Identity"] = errors.Select(e => e.Description).ToArray()
        };
        return new ValidationProblemDetails(errorDict)
        {
            Title  = "Identity Operation Failed",
            Detail = "One or more identity-related errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
    }
    
    public static ValidationProblemDetails ValidationProblem(IEnumerable<ValidationFailure> failures) 
        => new(FormatValidationErrors(failures))
        {
            Title  = "Validation Failed",
            Detail = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

    private static Dictionary<string, string[]> FormatValidationErrors(IEnumerable<ValidationFailure> failures)
        => failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );
    
    public static ProblemDetails ValidationProblem(ValidationFailure failure) 
        => new()
        {
            Title  = "Validation Failed",
            Detail = failure.ErrorMessage,
            Status = StatusCodes.Status400BadRequest
        };
    
    public static ProblemDetails Problem(string? message, int? statusCode) 
        => new()
        {
            Detail = message,
            Status = statusCode
        };
    
    public static ProblemDetails BadRequest(string? message) 
        => new()
        {
            Detail = message,
            Status = StatusCodes.Status400BadRequest
        };
    
    public static ProblemDetails NotFound(string? message) 
        => new()
        {
            Detail = message,
            Status = StatusCodes.Status404NotFound
        };
    
    public static ProblemDetails Unauthorized(string? message)
        => new()
        {
            Detail = message,
            Status = StatusCodes.Status401Unauthorized
        };
}