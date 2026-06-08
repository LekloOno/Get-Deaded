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
        set { _combineShapes = value; EmitSignal(SignalName.PropertyChanged); }
    }
    [Export] public FillData FillData
    {
        get => _fillData;
        set { _fillData = value; EmitSignal(SignalName.StructureChanged); }
    }
    [Export] public bool CombineOutlines
    {
        get => _combineOutlines;
        set { _combineOutlines = value; EmitSignal(SignalName.PropertyChanged); }
    }
    [Export] public OutlineData OutlineData
    {
        get => _outlineData;
        set { _outlineData = value; EmitSignal(SignalName.StructureChanged); }
    }
}