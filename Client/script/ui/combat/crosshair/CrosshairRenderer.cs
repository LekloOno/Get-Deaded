using System;
using Godot;

[Tool]
[GlobalClass]
public partial class CrosshairRenderer : Control
{
    [Export] private CrosshairData _defaultData = null!;
    private CrosshairData _data = null!;

    public event Action<CrosshairData, CrosshairData>? DataSwapped;

    [Export] public CrosshairData Data
    {
        get => _data;
        set
        {
            if (value == _data)
                return;
            
            var previous = _data;
            _data = value;
            DataSwapped?.Invoke(previous, value);
            QueueRedraw();
        }
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return;

        Data = CrosshairSetting.Instance.Data;
        CrosshairSetting.Instance.DataSwapped += SetData;
    }

    private void SetData(CrosshairData prev, CrosshairData current) =>
        Data = current;

    public override void _Draw()
    {
        if (Data == null)
            return;
               
        Vector2 center = Size / 2f;

        if (_data != null && _data.CombineShapes)
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

    private FillData ResolveFill(CrosshairShapeData shape) =>
        _data.CombineShapes ? _data.FillData : shape.FillData;

    private OutlineData ResolveOutline(CrosshairShapeData shape) => 
        _data.CombineOutlines ? _data.OutlineData : shape.OutlineData;

    private void DrawFill(CrosshairShapeData shape, Control canvas, Vector2 center)
    {
        if (shape == null)
            return;

        var fill = ResolveFill(shape);
        if (!fill.Visible)
            return;

        ApplyRotated(shape, canvas, center,
            () => shape.DrawFill(canvas, center, fill)
        );
    }

    private void DrawOutline(CrosshairShapeData shape, Control canvas, Vector2 center)
    {
        if (shape == null)
            return;

        var outline = ResolveOutline(shape);
        
        if (!outline.Visible)
            return;

        ApplyRotated(shape, canvas, center,
            () => shape.DrawOutline(canvas, center, outline)
        );
    }

    private static void ApplyRotated(CrosshairShapeData shape, Control canvas, Vector2 center, Action draw)
    {
        if (shape == null)
            return;

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