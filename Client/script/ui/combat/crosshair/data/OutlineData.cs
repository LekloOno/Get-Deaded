using Godot;

[Tool]
[GlobalClass]
public partial class OutlineData : FillData
{
    public OutlineData() { _color = Colors.Black; }
    private float _width = 1f;

    [Export] public float Width
    {
        get => _width;
        set { _width = value; EmitPropertyChanged(); }
    }
}