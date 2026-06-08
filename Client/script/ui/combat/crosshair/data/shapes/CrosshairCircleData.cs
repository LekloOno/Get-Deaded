using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CrosshairCircleData : CrosshairShapeData
{
    public float _radius     = 12f;
    public float _thickness  = 1f;
    public int   _points     = 64;
    public bool  _customArc  = false;
    public float _startAngle = 0f;
    public float _endAngle   = 360f;

    [Export(PropertyHint.Range, "0,30,or_greater")]
    public float Radius
    {
        get => _radius;
        set { _radius = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float Thickness
    {
        get => _thickness;
        set { _thickness = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "4,128,1")]
    public int Points
    {
        get => _points;
        set { _points = value; EmitPropertyChanged(); }
    }

    [Export] public bool CustomArc
    {
        get => _customArc;
        set
        {
            _customArc = value;
            NotifyPropertyListChanged();
            EmitPropertyChanged();
        }
    }

    [Export(PropertyHint.Range, "-360,360,0.5,degrees")]
    public float StartAngle
    {
        get => _startAngle;
        set { _startAngle = value; EmitPropertyChanged(); }
    }

    [Export(PropertyHint.Range, "-360,360,0.5,degrees")]
    public float EndAngle
    {
        get => _endAngle;
        set { _endAngle = value; EmitPropertyChanged(); }
    }

    public override void _ValidateProperty(Dictionary property)
    {
        var name = property["name"].AsString();
        if ((name == nameof(StartAngle) || name == nameof(EndAngle)) && !_customArc)
            property["usage"] = (int)PropertyUsageFlags.Storage;
    }

    private float ResolvedStart => _customArc ? Mathf.DegToRad(StartAngle) : 0f;
    private float ResolvedEnd   => _customArc ? Mathf.DegToRad(EndAngle)   : Mathf.Tau;

    public override void DrawFill(Control canvas, Vector2 center, FillData fill) =>
        canvas.DrawArc(center,
            Radius, ResolvedStart, ResolvedEnd, Points,
            fill.Color, Thickness, fill.AntiAlias);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        canvas.DrawArc(center,
            Radius, ResolvedStart, ResolvedEnd, Points,
            outline.Color, Thickness + outline.Width * 2f, outline.AntiAlias);
}