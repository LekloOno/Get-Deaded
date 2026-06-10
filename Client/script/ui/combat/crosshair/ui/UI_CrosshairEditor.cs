using Godot;

[GlobalClass]
public partial class UI_CrosshairEditor : Control
{
    private CrosshairData _data = null!;

    [Export] public CheckBox CombineShapes   { get; private set; } = null!;
    [Export] public CheckBox CombineOutlines { get; private set; } = null!;
    [Export] private UI_CrosshairFillData    _fill = null!;
    [Export] private UI_CrosshairOutlineData _outline = null!;

    [Export] private Container     _layersContainer = null!;
    [Export] private UI_CrosshairLayerOptionButton _addLayerOption = null!;
    [Export] private PackedScene   _shapeContainer = null!;

    public override void _Ready()
    {
        SetData(CrosshairSetting.Instance.Data);

        CombineShapes.Toggled   += OnCombineShapesToggled;
        CombineOutlines.Toggled += OnCombineOutlinesToggled;

        _addLayerOption.NewTypeRequested += OnNewLayerRequested;
        _addLayerOption.Select(-1);

        VisibilityChanged += OnVisibilityChanged;
    }

    private void SetData(CrosshairData data)
    {
        if (_data == data)
            return;

        if (_data != null)
        {
            _data.StructureChanged -= OnStructureChanged;
            _data.LayerMovedUp     -= OnLayerMoved;
            _data.LayerMovedDown   -= OnLayerMoved;
            _data.LayerMovedTo     -= OnLayerMoved;
        }

        _data = data;

        if (_data == null)
            return;

        OnStructureChanged();
        _data.StructureChanged += OnStructureChanged;
        _data.LayerMovedUp     += OnLayerMoved;
        _data.LayerMovedDown   += OnLayerMoved;
        _data.LayerMovedTo     += OnLayerMoved;
    }

    private void OnVisibilityChanged()
    {
        if (IsVisibleInTree())
            SetData(CrosshairSetting.Instance.Data);
        else
            CrosshairSetting.Instance.Save(_data);
    }

    private void OnStructureChanged()
    {
        Clear();

        foreach (CrosshairShapeData shape in _data.Shapes)
            AddNewLayerUi(shape);

        CombineShapes.SetPressedNoSignal(_data.CombineShapes);
        CombineOutlines.SetPressedNoSignal(_data.CombineOutlines);

        _data.OutlineData.Visible = _data.CombineOutlines;
        _data.FillData.Visible = _data.CombineShapes;

        _outline.SetOutlineData(_data.OutlineData);
        _fill.SetData(_data.FillData);
    }

    private void OnCombineShapesToggled(bool toggledOn) =>
        _data.CombineShapes = toggledOn;

    private void OnCombineOutlinesToggled(bool toggledOn) =>
        _data.CombineOutlines = toggledOn;

    private void OnNewLayerRequested(CrosshairShapeData shape)
    {
        _addLayerOption.Select(-1);
        _data.AddShape(shape);
        AddNewLayerUi(shape);
    }

    private void AddNewLayerUi(CrosshairShapeData shape)
    {
        UI_CrosshairShapeContainer shapeContainer = _shapeContainer.Instantiate<UI_CrosshairShapeContainer>();
        shapeContainer.SetData(_data, shape);
        _layersContainer.AddChild(shapeContainer);
    }

    private void OnLayerMoved(CrosshairShapeData shape, int from, int to)
    {
        Node shapeContainer = _layersContainer.GetChild(from);
        _layersContainer.MoveChild(shapeContainer, to);
    }

    private void Clear()
    {
        foreach (Node child in _layersContainer.GetChildren())
            child.QueueFree();
    }
}