using Godot;

[GlobalClass]
public partial class CrosshairSquareData : CrosshairShapeData
{
    [Export] public float Size      { get; set; } = 8f;
    [Export] public bool  Filled    { get; set; } = false;
    [Export] public float Thickness { get; set; } = 1.5f; // if Filled is false

    public override void DrawFill(Control canvas, Vector2 center, Color color)
    {
        var rect = MakeRect(center, Size);
        if (Filled)
            canvas.DrawRect(rect, color, true);
        else
            canvas.DrawRect(rect, color, false, Thickness);
    }

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline)
    {
        var rect = MakeRect(center, Size + outline.Width * 2f);
        if (Filled)
            canvas.DrawRect(rect, outline.Color, true);
        else
            canvas.DrawRect(rect, outline.Color, false, Thickness + outline.Width * 2f);
    }

    private static Rect2 MakeRect(Vector2 center, float size) =>
        new(center - Vector2.One * size / 2f, Vector2.One * size);
}