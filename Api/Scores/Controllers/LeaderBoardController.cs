using Api.Scores.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Scores;

namespace Api.Scores.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboard;
    public LeaderboardController(ILeaderboardService leaderboard) => _leaderboard = leaderboard;

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardRowDto>>> Get(
        string mapKey, Difficulty difficulty, int centerRank, int take, CancellationToken ct)
    {
        take = Math.Clamp(take, 1, 100);
        return Ok(await _leaderboard.GetWindowAsync(new LeaderboardScope(mapKey, difficulty), centerRank, take, ct));
    }

    [HttpGet("around/{scoreId:guid}")]
    public async Task<ActionResult<List<LeaderboardRowDto>>> GetAroundScore(Guid scoreId, int take, CancellationToken ct)
    {
        take = Math.Clamp(take, 1, 100);
        return Ok(await _leaderboard.GetAroundScoreAsync(scoreId, take, ct));
    }
}