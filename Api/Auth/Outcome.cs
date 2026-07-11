using Shared.Auth;

namespace Api.Auth;

public enum AuthOutcomeStatus { Success, InvalidCredentials, UsernameTaken }
public record AuthOutcome(AuthOutcomeStatus Status, AuthResponse? Response = null);