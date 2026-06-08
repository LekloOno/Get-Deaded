using Godot;

[GlobalClass]
public partial class CrosshairDotData : CrosshairShapeData
{
    [Export] public float Radius { get; set; } = 2f;

    public override void DrawFill(Control canvas, Vector2 center, Color color) =>
        canvas.DrawCircle(center, Radius, color, true);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        canvas.DrawCircle(center, Radius + outline.Width, outline.Color, true);
}