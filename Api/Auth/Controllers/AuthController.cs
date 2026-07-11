using Api.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shared.Auth;

namespace Api.Auth.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var outcome = await _auth.RegisterAsync(request, ct);
        return outcome.Status switch
        {
            AuthOutcomeStatus.UsernameTaken => Conflict("Username already exists."),
            AuthOutcomeStatus.Success => Ok(outcome.Response),
            _ => StatusCode(500)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var outcome = await _auth.LoginAsync(request, ct);
        return outcome.Status switch
        {
            AuthOutcomeStatus.InvalidCredentials => Unauthorized(),
            AuthOutcomeStatus.Success => Ok(outcome.Response),
            _ => StatusCode(500)
        };
    }
}