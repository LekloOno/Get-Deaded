using Godot;

[Tool]
[GlobalClass]
public partial class CrosshairDotData : CrosshairShapeData
{
    private float _radius = 5f;

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Radius
    {
        get => _radius;
        set { _radius = value; EmitPropertyChanged(); }
    }

    public override void DrawFill(Control canvas, Vector2 center, FillData fill) =>
        canvas.DrawCircle(center, Radius, fill.Color, true, -1, fill.AntiAlias);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        canvas.DrawCircle(center, Radius + outline.Width, outline.Color, true, -1, outline.AntiAlias);
}