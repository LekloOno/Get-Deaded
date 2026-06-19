using Godot;

public partial class UI_Error : Control
{
    [Export] private Label _messageLabel = null!;

    public void ShowError(string message)
    {
        _messageLabel.Text = message;
        Show();
    }
}