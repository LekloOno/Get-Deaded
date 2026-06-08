using System;
using Godot;

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

    [Export] public ArmMask Arms      { get; set; } = ArmMask.Top | ArmMask.Bottom | ArmMask.Left | ArmMask.Right;
    [Export] public float   Size      { get; set; } = 10f;
    [Export] public float   Gap       { get; set; } = 4f;
    [Export] public float   Thickness { get; set; } = 2f;

    public override void DrawFill(Control canvas, Vector2 center, Color color) =>
        DrawLines(canvas, center, color, Thickness);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        DrawLines(canvas, center, outline.Color, Thickness + outline.Width * 2f);

    private void DrawLines(Control canvas, Vector2 c, Color color, float thick)
    {
        float g = Gap, s = Size;
        if ((Arms & ArmMask.Top)    != 0) canvas.DrawLine(c + new Vector2(0,  -g), c + new Vector2(0,  -g - s), color, thick, true);
        if ((Arms & ArmMask.Bottom) != 0) canvas.DrawLine(c + new Vector2(0,   g), c + new Vector2(0,   g + s), color, thick, true);
        if ((Arms & ArmMask.Left)   != 0) canvas.DrawLine(c + new Vector2(-g,  0), c + new Vector2(-g - s,  0), color, thick, true);
        if ((Arms & ArmMask.Right)  != 0) canvas.DrawLine(c + new Vector2( g,  0), c + new Vector2( g + s,  0), color, thick, true);
    }
}