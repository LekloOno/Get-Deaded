using System;
using Godot;

[Tool]
[GlobalClass]
public partial class CrosshairCrossData : CrosshairShapeData
{
    [Flags] public enum ArmMask
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    private float _size      = 8f;
    private float _gap       = 4f;
    private float _thickness = 1f;
    private ArmMask _arms = ArmMask.Top | ArmMask.Bottom | ArmMask.Left | ArmMask.Right;

    [Export] public ArmMask Arms
    {
        get => _arms;
        set { _arms = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "0,30,or_greater")]
    public float Length
    {
        get => _size;
        set { _size = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Gap
    {
        get => _gap;
        set { _gap = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Thickness
    {
        get => _thickness;
        set { _thickness = value; EmitPropertyChanged(); }
    }

    public override void DrawFill(Control canvas, Vector2 center, FillData fill) =>
        DrawLines(canvas, center, Length, Gap, fill.Color, Thickness, fill.AntiAlias);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        DrawLines(canvas, center, Length + outline.Width * 2f, Gap - outline.Width, outline.Color, Thickness + outline.Width * 2f, outline.AntiAlias);

    private void DrawLines(Control canvas, Vector2 c, float s, float g, Color color, float thick, bool antiAlias)
    {
        if ((Arms & ArmMask.Top)    != 0) canvas.DrawLine(c + new Vector2(0,  -g), c + new Vector2(0,  -g - s), color, thick, antiAlias);
        if ((Arms & ArmMask.Bottom) != 0) canvas.DrawLine(c + new Vector2(0,   g), c + new Vector2(0,   g + s), color, thick, antiAlias);
        if ((Arms & ArmMask.Left)   != 0) canvas.DrawLine(c + new Vector2(-g,  0), c + new Vector2(-g - s,  0), color, thick, antiAlias);
        if ((Arms & ArmMask.Right)  != 0) canvas.DrawLine(c + new Vector2( g,  0), c + new Vector2( g + s,  0), color, thick, antiAlias);
    }
}