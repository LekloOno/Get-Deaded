using Godot;

[GlobalClass]
public partial class OutlineData : Resource
{
    [Export] public bool  Visible { get; set; } = true;
    [Export] public Color Color   { get; set; } = Colors.Black;
    [Export] public float Width   { get; set; } = 1f;

    public OutlineData(){}
    public OutlineData(bool visible, Color color, float width)
        => (Visible, Color, Width) = (visible, color, width);
}