using System;
using Godot;

[GlobalClass]
public partial class CrosshairRenderer : Control
{
    private CrosshairData _data = null!;

    [Export] public CrosshairData Data
    {
        get => _data;
        set
        {
            if (value == _data)
                return;
            
            _data = value;
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        Vector2 center = Size / 2f;

        if (_data.CombineShapes)
            DrawCombined(center);
        else
            DrawIndependant(center);
    }

    private void DrawIndependant(Vector2 center)
    {
        foreach (CrosshairShapeData shape in _data.Shapes)
        {
            DrawOutline(shape, this, center);
            DrawFill(shape, this, center);
        }
    }

    private void DrawCombined(Vector2 center)
    {
        // We could early skip outline draw if combined + not visible, but it's a riddiculous gain
        // Might as well just keep the code super easy to read
        foreach (CrosshairShapeData shape in _data.Shapes)
            DrawOutline(shape, this, center);

        foreach (CrosshairShapeData shape in _data.Shapes)
            DrawFill(shape, this, center);
    }

    private Color ResolveFillColor(CrosshairShapeData shape) =>
        _data.CombineShapes ? _data.FillColor : shape.FillColor;

    private OutlineData ResolveOutline(CrosshairShapeData shape) => 
        _data.CombineOutlines ? _data.OutlineData : shape.OutlineData;

    private void DrawFill(CrosshairShapeData shape, Control canvas, Vector2 center)
    {
        var color = ResolveFillColor(shape);
        ApplyRotated(shape, canvas, center,
            () => shape.DrawFill(canvas, center, color)
        );
    }

    private void DrawOutline(CrosshairShapeData shape, Control canvas, Vector2 center)
    {
        var outline = ResolveOutline(shape);
        
        if (!outline.Visible)
            return;

        ApplyRotated(shape, canvas, center,
            () => shape.DrawOutline(canvas, center, outline)
        );
    }

    private static void ApplyRotated(CrosshairShapeData shape, Control canvas, Vector2 center, Action draw)
    {
        if (shape.RotationDegrees == 0f)
        {
            draw();
            return;
        }

        float rad = Mathf.DegToRad(shape.RotationDegrees);
        canvas.DrawSetTransform(center, rad, Vector2.One);
        draw();
        canvas.DrawSetTransform(Vector2.Zero, 0f, Vector2.One); // restore to identity
    }
}