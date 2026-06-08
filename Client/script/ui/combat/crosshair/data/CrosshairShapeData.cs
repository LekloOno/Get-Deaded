using Godot;

[Tool]
[GlobalClass]
public abstract partial class CrosshairShapeData : Resource
{
    [Signal] public delegate void PropertyChangedEventHandler();

    private FillData _fillData = new();
    private float _rotationDegrees   = 0f;
    private OutlineData _outlineData = new();


    [Export] public FillData FillData
    {
        get => _fillData;
        set
        {
            _fillData = value;
            EmitPropertyChanged();
        }
    }
    [Export(PropertyHint.Range, "-180,180,0.5,degrees")]
    public float RotationDegrees
    {
        get => _rotationDegrees;
        set { _rotationDegrees = value; EmitPropertyChanged(); }
    }
    [Export] public OutlineData OutlineData
    {
        get => _outlineData;
        set
        {
            _outlineData = value;
            EmitPropertyChanged();
        }
    }

    protected void EmitPropertyChanged() => EmitSignal(SignalName.PropertyChanged);
    
    public abstract void DrawFill(Control canvas, Vector2 center, FillData fill);
    public abstract void DrawOutline(Control canvas, Vector2 center, OutlineData outline);
}