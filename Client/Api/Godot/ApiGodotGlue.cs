using Godot;
using Shared.Scores;
using Client.Api.Auth;
using Client.Api.Score;
using System.Threading.Tasks;
using Godot.Collections;

namespace Client.Api.Godot;

public partial class ApiGodotGlue : Node
{
    public static ApiGodotGlue? Instance {get; private set;}
    public AuthApi AuthApi {get; private set;} = new();
    public ScoreApi ScoreApi {get; private set;} = new();

    [Signal]
    public delegate void LoginFinishedEventHandler(Dictionary result);

    [Signal]
    public delegate void RegisterFinishedEventHandler(Dictionary result);

    // No signals for scores, they are a little more cumbersome to glue to gdscript
    // since I must share the DTO. I'll keep in pure c# for now.
    // Don't think it's worth also wrapping the DTO..

    public override void _EnterTree()
    {
        Instance = this;
    }

    public async void Login(string username, string password)
    {
        AuthResult result = await AuthApi.LoginAsync(username, password);

        EmitSignal(SignalName.LoginFinished, result.ToGdPayload());
    }

    public async void Register(string username, string password)
    {
        AuthResult result = await AuthApi.RegisterAsync(username, password);

        EmitSignal(SignalName.RegisterFinished, result.ToGdPayload());
    }

    public async Task<ScoreResult> SubmitScore(SubmitScoreRequest score) =>
        await ScoreApi.SubmitScoreAsync(score);
}