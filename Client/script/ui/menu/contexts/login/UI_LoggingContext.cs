using Client.Api.Auth;
using Godot;

public partial class UI_LoggingContext : Control
{
    [Export] private bool _logged = false;

    public override void _Ready()
    {
        Session.Token.Subscribe(UpdateUi);
        UpdateUi(null);
    }

    private void UpdateUi(string? _)
    {
        Visible = Session.IsAuthenticated == _logged;
    }
}