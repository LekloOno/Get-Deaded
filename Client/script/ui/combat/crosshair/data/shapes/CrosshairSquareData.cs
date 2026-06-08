using Godot;

[Tool]
[GlobalClass]
public partial class CrosshairSquareData : CrosshairShapeData
{
    private float _size      = 8f;
    private bool  _filled    = false;
    private float _thickness = 1.5f;

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Size
    {
        get => _size;
        set { _size = value; EmitPropertyChanged(); }
    }

    [Export] public bool  Filled 
    {
        get => _filled;
        set { _filled = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Thickness
    {
        get => _thickness;
        set { _thickness = value; EmitPropertyChanged(); }
    }

    public override void DrawFill(Control canvas, Vector2 center, FillData fill)
    {
        var rect = MakeRect(center, Size);
        if (Filled)
            canvas.DrawRect(rect, fill.Color, true, -1, fill.AntiAlias);
        else
            canvas.DrawRect(rect, fill.Color, false, Thickness, fill.AntiAlias);
    }

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline)
    {
        var rect = MakeRect(center, Size + outline.Width * 2f);
        if (Filled)
            canvas.DrawRect(rect, outline.Color, true, -1, outline.AntiAlias);
        else
            canvas.DrawRect(rect, outline.Color, false, Thickness + outline.Width * 2f, outline.AntiAlias);
    }

    private static Rect2 MakeRect(Vector2 center, float size) =>
        new(center - Vector2.One * size / 2f, Vector2.One * size);
}