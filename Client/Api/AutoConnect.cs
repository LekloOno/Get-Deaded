using Godot;
using Client.Api.Auth;

namespace Client.Api;

public partial class AutoConnect : Node
{
    public override void _Ready()
    {
        Connect();        
    }

    public static async void Connect()
    {
        var authApi = new AuthApi();

        var success = await authApi.LoginAsync(
            "test",
            "cestca");
    }
}