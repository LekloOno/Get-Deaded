using Godot;

[Tool]
[GlobalClass]
public partial class FillData : Resource
{
    [Signal] public delegate void PropertyChangedEventHandler();

    protected Color _color  = Colors.White;
    private bool _visible   = true;
    private bool _antiAlias = true;

    [Export] public bool Visible
    {
        get => _visible;
        set { _visible = value; EmitPropertyChanged(); }
    }
    [Export] public Color Color
    {
        get => _color;
        set { _color = value; EmitPropertyChanged(); }
    }

    [Export] public bool AntiAlias
    {
        get => _antiAlias;
        set { _antiAlias = value; EmitPropertyChanged(); }
    }

    protected void EmitPropertyChanged() => EmitSignal(SignalName.PropertyChanged);
}