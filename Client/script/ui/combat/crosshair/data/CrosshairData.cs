using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CrosshairData : Resource
{
    [Signal] public delegate void PropertyChangedEventHandler();
    [Signal] public delegate void StructureChangedEventHandler();
    [Signal] public delegate void ShapeAddedEventHandler(CrosshairShapeData shape);
    [Signal] public delegate void ShapeRemovedEventHandler(CrosshairShapeData shape);
    [Signal] public delegate void LayerMovedUpEventHandler(CrosshairShapeData shape, int from, int to);
    [Signal] public delegate void LayerMovedDownEventHandler(CrosshairShapeData shape, int from, int to);
    [Signal] public delegate void LayerMovedToEventHandler(CrosshairShapeData shape, int from, int to);

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
            EmitSignal(SignalName.ShapeRemoved, shape);
    }

    public void AddShape(CrosshairShapeData shape)
    {
        _shapes.Add(shape);
        EmitSignal(SignalName.ShapeAdded, shape);
    }

    public void MoveLayerUp(int from)
    {
        int to = from - 1;
        if (to < 0 || from >= _shapes.Count)
            return;

        (_shapes[to], _shapes[from]) =
            (_shapes[from], _shapes[to]);

        EmitSignal(SignalName.LayerMovedUp, _shapes[to], from, to);
    }

    public void MoveLayerDown(int from)
    {
        int to = from + 1;
        if (from < 0 || to >= _shapes.Count)
            return;

        (_shapes[from], _shapes[to]) =
            (_shapes[to], _shapes[from]);

        EmitSignal(SignalName.LayerMovedDown, _shapes[to], from, to);
    }

    public void MoveLayerTo(int from, int to)
    {
        if (from == to)
            return;

        CrosshairShapeData layer = _shapes[from];
        _shapes.RemoveAt(from);
        _shapes.Insert(to, layer);

        EmitSignal(SignalName.LayerMovedTo, layer, from, to);
    }
}