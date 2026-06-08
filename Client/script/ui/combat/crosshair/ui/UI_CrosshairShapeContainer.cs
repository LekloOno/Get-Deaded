using Godot;

[GlobalClass]
public partial class UI_CrosshairShapeContainer : Control
{
    private CrosshairData _crosshair = null!;
    private CrosshairShapeData _shape = null!;

    [Export] private Button     _deleteButton = null!;
    [Export] private Container  _shapeSettingsContainer = null!;
    [Export] private Range      _rotation = null!;
    [Export] private UI_CrosshairFillData    _fill = null!;
    [Export] private UI_CrosshairOutlineData _outlines = null!;

    [Export] private PackedScene _dotScene      = null!;
    [Export] private PackedScene _circleScene   = null!;
    [Export] private PackedScene _crossScene    = null!;
    [Export] private PackedScene _squareScene   = null!;

    public override void _Ready()
    {
        _rotation.ValueChanged += OnRotationChanged;
        _deleteButton.Pressed += OnDeletePressed;
    }

    public void SetData(CrosshairData crosshair, CrosshairShapeData shape)
    {
        _shape = shape;

        _rotation.Value = _shape.RotationDegrees;

        _fill.SetData(_shape.FillData);
        _outlines.SetOutlineData(_shape.OutlineData);


        SetCrosshair(crosshair);

        Clear();

        Control shapeSettings = CreateShapeSettings(shape);
        _shapeSettingsContainer.AddChild(shapeSettings);
    }

    private void SetCrosshair(CrosshairData crosshair)
    {
        if (crosshair == _crosshair)
            return;

        if (_crosshair != null)
            _crosshair.PropertyChanged -= UpdateVisibility;
        
        _crosshair = crosshair;
        
        UpdateVisibility();
        _crosshair.PropertyChanged += UpdateVisibility;
    }

    private void UpdateVisibility()
    {
        _outlines.Visible = !_crosshair.CombineOutlines;
        _fill.Visible = !_crosshair.CombineShapes;
    }

    private Control CreateShapeSettings(CrosshairShapeData shape)
    {
        return shape switch
        {
            CrosshairDotData    dot    => CreateDotSettings(dot),
            CrosshairCrossData  cross  => CreateCrossSettings(cross),
            CrosshairCircleData circle => CreateCircleSettings(circle),
            CrosshairSquareData square => CreateSquareSettings(square),
            _ => throw new System.NotImplementedException(),
        };
    }

    private UI_CrosshairDot CreateDotSettings(CrosshairDotData data)
    {
        UI_CrosshairDot dot = _dotScene.Instantiate<UI_CrosshairDot>();
        dot.SetData(data);
        return dot;
    }

    private UI_CrosshairCross CreateCrossSettings(CrosshairCrossData data)
    {
        UI_CrosshairCross cross = _crossScene.Instantiate<UI_CrosshairCross>();
        cross.SetData(data);
        return cross;
    }

    private UI_CrosshairCircle CreateCircleSettings(CrosshairCircleData data)
    {
        UI_CrosshairCircle circle = _crossScene.Instantiate<UI_CrosshairCircle>();
        circle.SetData(data);
        return circle;
    }

    private UI_CrosshairSquare CreateSquareSettings(CrosshairSquareData data)
    {
        UI_CrosshairSquare square = _crossScene.Instantiate<UI_CrosshairSquare>();
        square.SetData(data);
        return square;
    }

    private void OnRotationChanged(double value) =>
        _shape.RotationDegrees = (float) value;

    private void OnDeletePressed()
    {
        _crosshair.RemoveShape(_shape);
        QueueFree();
    }

    private void Clear()
    {
        foreach (Node child in _shapeSettingsContainer.GetChildren())
            child.QueueFree();
    }
}