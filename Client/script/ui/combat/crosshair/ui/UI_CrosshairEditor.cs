using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_CrosshairEditor : Control
{
    [Export] private CrosshairData _data = null!;

    [Export] public CheckBox CombineShapes   { get; private set; } = null!;
    [Export] public CheckBox CombineOutlines { get; private set; } = null!;
    [Export] private UI_CrosshairFillData    _fill = null!;
    [Export] private UI_CrosshairOutlineData _outline = null!;

    [Export] private Container     _layersContainer = null!;
    [Export] private UI_CrosshairLayerOptionButton _addLayerOption = null!;
    [Export] private PackedScene   _shapeContainer = null!;

    public override void _Ready()
    {
        Clear();

        CombineShapes.ButtonPressed    = _data.CombineShapes;
        CombineOutlines.ButtonPressed  = _data.CombineOutlines;

        _fill.SetData(_data.FillData);
        _outline.SetData(_data.OutlineData);

        UpdateVisibility();

        foreach (CrosshairShapeData shape in _data.Shapes)
            AddNewLayerUi(shape);

        _data.StructureChanged  += OnStructureChanged; 

        CombineShapes.Toggled   += OnCombineShapesToggled;
        CombineOutlines.Toggled += OnCombineOutlinesToggled;

        _addLayerOption.NewLayerRequested += OnNewLayerRequested;
    }

    private void OnStructureChanged()
    {
        Clear();
        foreach (CrosshairShapeData shape in _data.Shapes)
            AddNewLayerUi(shape);

        _fill.SetData(_data.FillData);
        _outline.SetData(_data.OutlineData);
    }

    private void UpdateVisibility()
    {
        _fill.Visible = _data.CombineShapes;
        _outline.Visible = _data.CombineOutlines;
    }

    private void OnCombineShapesToggled(bool toggledOn) =>
        _data.CombineShapes = toggledOn;

    private void OnCombineOutlinesToggled(bool toggledOn) =>
        _data.CombineOutlines = toggledOn;

    private void OnNewLayerRequested(CrosshairShapeData shape)
    {
        _data.AddShape(shape);
        AddNewLayerUi(shape);
    }

    private void AddNewLayerUi(CrosshairShapeData shape)
    {
        UI_CrosshairShapeContainer shapeContainer = _shapeContainer.Instantiate<UI_CrosshairShapeContainer>();
        shapeContainer.SetData(_data, shape);
        _layersContainer.AddChild(shapeContainer);
    }

    private void Clear()
    {
        foreach (Node child in _layersContainer.GetChildren())
            child.QueueFree();
    }
}