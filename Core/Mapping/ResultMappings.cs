using Microsoft.AspNetCore.Mvc;

namespace Core.Mapping;

public static class ResultMappings
{
    public static ObjectResult ToObjectResult(this ProblemDetails problemDetails) 
        => new(problemDetails);
}