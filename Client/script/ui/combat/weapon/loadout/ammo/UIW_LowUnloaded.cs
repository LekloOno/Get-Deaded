using Godot;

[GlobalClass]
public partial class UIW_LowUnloaded : Control
{
    [Export] private Label? _label;
    [Export] private Label? _emptyLabel;

    public void SetCritical()
    {
        _emptyLabel?.Hide();
        if (_label == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _label.");
            return;
        }
        
        _label.Show();
        _label.ThemeTypeVariation = "LowUnloadedCritical";
    }

    public void SetNormal()
    {
        _emptyLabel?.Hide();
        if (_label == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _label.");
            return;
        }
        
        _label.Show();
        _label.ThemeTypeVariation = "LowUnloaded";
    }

    public void SetEmpty()
    {
        _label?.Hide();
        if (_emptyLabel == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _emtpyLabel.");
            return;
        }
        _emptyLabel?.Show();
    } 
}