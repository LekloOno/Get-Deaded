using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Scores.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Scores;

namespace Api.Scores.Controllers;

[ApiController]
[Route("api/scores")]
public class ScoresController : ControllerBase
{
    private readonly IScoreService _scores;
    public ScoresController(IScoreService scores) => _scores = scores;

    [Authorize]
    [HttpPost]
    [EnableRateLimiting("submit-score")]
    public async Task<ActionResult<SubmitScoreResponse>> Submit(SubmitScoreRequest request, CancellationToken ct)
    {
        var playerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        return Ok(await _scores.SubmitAsync(playerId, request, ct));
    }

    [HttpGet("{id:guid}")]
    [EnableRateLimiting("score-detail")]
    public async Task<ActionResult<ScoreDto>> GetDetail(Guid id, CancellationToken ct)
    {
        var score = await _scores.GetDetailAsync(id, ct);
        return score is null ? NotFound() : Ok(score);
    }
}