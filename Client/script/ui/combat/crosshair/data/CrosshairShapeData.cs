using Godot;

[GlobalClass]
public abstract partial class CrosshairShapeData : Resource
{
    [Export] public Color FillColor  { get; set; } = Colors.White;
    [Export] public OutlineData OutlineData { get; set; } = new();
    [Export(PropertyHint.Range, "-180,180,0.5,degrees")] 
    public float RotationDegrees { get; set; } = 0f;
    
    public abstract void DrawFill(Control canvas, Vector2 center, Color color);
    public abstract void DrawOutline(Control canvas, Vector2 center, OutlineData outline);
}