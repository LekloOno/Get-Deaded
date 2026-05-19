using Client.Api.Auth;
using Client.Api.Score;
using Godot.Collections;

namespace Client.Api.Godot;

public static class ResultExtensions
{
    public static Dictionary ToGdPayload(this AuthResult result)
    {
        return new Dictionary
        {
            ["success"] = result.Success,
            ["error"] = result.Error.ToString(),
            ["message"] = result.Message ?? "",
            ["token"] = result.Token ?? ""
        };
    }

    public static Dictionary ToGdPayload(this ScoreResult result)
    {
        return new Dictionary
        {
            ["success"] = result.Success,
            ["error"] = result.Error.ToString(),
            ["message"] = result.Message ?? "",
            ["score_id"] = result.ScoreId?.ToString() ?? "",
            ["rank"] = result.Rank ?? -1
        };
    }
}