using Microsoft.AspNetCore.Mvc;

namespace Core.Results;

public readonly struct NoContent
{
    public static readonly NoContent Value = new();
}
public readonly record struct Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private TValue         value   { get; }
    private ProblemDetails problem { get; }
    
    public static implicit operator Result<TValue>(TValue         value)   => new(value);
    public static implicit operator Result<TValue>(ProblemDetails problem) => new(problem);
    
    public static Result<TValue> Success(TValue        value)   => new(value);
    public static Result<TValue> Failed(ProblemDetails problem) => new(problem);
    
    public TValue         Value   => IsSuccess  ? value   : throw new InvalidOperationException("Cannot get value of failed result.");
    public ProblemDetails Problem => !IsSuccess ? problem : throw new InvalidOperationException("Cannot get problem of successful result.");
    
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess, Func<ProblemDetails, TResult> onFailure) 
        => IsSuccess ? onSuccess(value) : onFailure(problem);

    public void Perform(Action onSuccess, Action onFailure)
    {
        if (IsSuccess)
        {
            onSuccess();
        }
        else
        {
            onFailure();
        }
    }
    public Result(TValue value)
    {
        this.value = value;
        problem    = null!;
        IsSuccess  = true;
    }
    public Result(ProblemDetails problem)
    {
        value        = default!;
        this.problem = problem;
        IsSuccess    = false;
    }
    
    public void AssertSuccess()
    {
        if (IsFailure)
        {
            throw new InvalidOperationException("Expected success but got failure: " + problem?.Detail);
        }
    }
    public void AssertFailure()
    {
        if (IsSuccess)
        {
            throw new InvalidOperationException("Expected failure but got success.");
        }
    }
}