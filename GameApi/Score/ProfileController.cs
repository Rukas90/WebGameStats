using Core.Mapping;
using Core.Security;
using GameApi.Requests.Score;
using GameApi.Services.Score;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Endpoints;

[ApiController]
[Route("v1/game/profile")]
public class ProfileController(IScoreService scoreService, IAntiforgery antiforgery) : ControllerBase
{
    [Authorize]
    [HttpPost("score")]
    public async Task<IActionResult> UpdateScore([FromBody] ScoreUpdateRequest request)
    {
        await antiforgery.ValidateRequestAsync(HttpContext);
        
        var idResult = JwtReader.GetUserId(User);
        
        if (idResult.IsFailure)
        {
            return idResult.Problem.ToObjectResult();
        }
        var userId = idResult.Value;

        return (await scoreService.UpdateScoreAsync(userId, request.Amount))
            .Match(
                onFailure: problem => problem.ToObjectResult(),
                onSuccess: newScore => Ok(new ScoreUpdatedResponse(newScore)));
    }
}