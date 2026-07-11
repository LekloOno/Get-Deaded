using Shared.Auth;

namespace Api.Auth;

public interface IAuthService
{
    Task<AuthOutcome> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthOutcome> LoginAsync(LoginRequest request, CancellationToken ct);
}