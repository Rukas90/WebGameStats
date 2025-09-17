using IdentityApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services;

internal readonly record struct UserAuthValidationResult
{
    public bool            IsAuthenticated { get; init; }
    public ProblemDetails? Problem         { get; init; }
    public User?           User            { get; init; }

    public static implicit operator UserAuthValidationResult(ProblemDetails problem) => Error(problem); 
    public static implicit operator UserAuthValidationResult(User           user)    => Success(user);
    
    public static UserAuthValidationResult Error(ProblemDetails problem) 
        => new() { IsAuthenticated = false, Problem = problem, User = null };
    
    public static UserAuthValidationResult Success(User user) 
        => new() { IsAuthenticated = true, Problem = null, User = user };
}