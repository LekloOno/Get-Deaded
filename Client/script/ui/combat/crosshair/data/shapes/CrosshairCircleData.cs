using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CrosshairCircleData : CrosshairShapeData
{
    [Export] public float Radius    { get; set; } = 12f;
    [Export] public float Thickness { get; set; } = 1.5f;
    [Export] public int   Points    { get; set; } = 64;

    private bool _customArc = false;
    [Export] public bool CustomArc
    {
        get => _customArc;
        set
        {
            _customArc = value;
            NotifyPropertyListChanged();
        }
    }

    [Export(PropertyHint.Range, "-360,360,0.5,degrees")]
    public float StartAngle { get; set; } = 0f;
    [Export(PropertyHint.Range, "-360,360,0.5,degrees")]
    public float EndAngle   { get; set; } = 360f;

    public override void _ValidateProperty(Dictionary property)
    {
        var name = property["name"].AsString();
        if ((name == nameof(StartAngle) || name == nameof(EndAngle)) && !_customArc)
            property["usage"] = (int)PropertyUsageFlags.Storage;
    }

    private float ResolvedStart => _customArc ? Mathf.DegToRad(StartAngle) : 0f;
    private float ResolvedEnd   => _customArc ? Mathf.DegToRad(EndAngle)   : Mathf.Tau;

    public override void DrawFill(Control canvas, Vector2 center, Color color) =>
        canvas.DrawArc(center,
            Radius, ResolvedStart, ResolvedEnd, Points,
            color, Thickness, true);

    public override void DrawOutline(Control canvas, Vector2 center, OutlineData outline) =>
        canvas.DrawArc(center,
            Radius, ResolvedStart, ResolvedEnd, Points,
            outline.Color, Thickness + outline.Width * 2f, true);
}