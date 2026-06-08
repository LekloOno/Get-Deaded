using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CrosshairData : Resource
{
    [Signal] public delegate void PropertyChangedEventHandler();
    [Signal] public delegate void StructureChangedEventHandler();

    private bool        _combineShapes   = true;
    private FillData    _fillData        = new();
    private bool        _combineOutlines = true;
    private OutlineData _outlineData     = new();
    private Array<CrosshairShapeData> _shapes          = [];

    [Export] public Array<CrosshairShapeData> Shapes
    {
        get => _shapes;
        set { _shapes = value; EmitSignal(SignalName.StructureChanged); }
    }
    [Export] public bool CombineShapes
    {
        get => _combineShapes;
        set { _combineShapes = value; NotifyPropertyListChanged(); EmitSignal(SignalName.PropertyChanged); }
    }
    [Export] public FillData FillData
    {
        get => _fillData;
        set { _fillData = value; EmitSignal(SignalName.StructureChanged); }
    }
    [Export] public bool CombineOutlines
    {
        get => _combineOutlines;
        set { _combineOutlines = value; NotifyPropertyListChanged(); EmitSignal(SignalName.PropertyChanged); }
    }
    [Export] public OutlineData OutlineData
    {
        get => _outlineData;
        set { _outlineData = value; EmitSignal(SignalName.StructureChanged); }
    }

    public override void _ValidateProperty(Dictionary property)
    {
        var name = property["name"].AsString();
        if ((name == nameof(FillData)) && !_combineShapes)
            property["usage"] = (int)PropertyUsageFlags.Storage;
        else if ((name == nameof(OutlineData)) && !_combineOutlines)
            property["usage"] = (int)PropertyUsageFlags.Storage;
    }

    public void RemoveShape(CrosshairShapeData shape)
    {
        if (_shapes.Remove(shape))
            EmitSignal(SignalName.StructureChanged);
    }

    public void AddShape(CrosshairShapeData shape)
    {
        _shapes.Add(shape);
        EmitSignal(SignalName.StructureChanged);
    }
}