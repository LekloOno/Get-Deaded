using Godot;

[GlobalClass]
public partial class UI_CrosshairShapeContainer : Control
{
    private CrosshairData _crosshair = null!;
    private CrosshairShapeData _shape = null!;

    [Export] private Button     _deleteButton = null!;
    [Export] private Button     _moveUpButton = null!;
    [Export] private Button     _moveDownButton = null!;
    [Export] private Container  _shapeSettingsContainer = null!;
    [Export] private Range      _rotation = null!;
    [Export] private UI_CrosshairFillData          _fill = null!;
    [Export] private UI_CrosshairOutlineData       _outlines = null!;
    [Export] private UI_CrosshairLayerOptionButton _typeOption = null!;

    [Export] private PackedScene _dotScene      = null!;
    [Export] private PackedScene _circleScene   = null!;
    [Export] private PackedScene _crossScene    = null!;
    [Export] private PackedScene _squareScene   = null!;

    public override void _Ready()
    {
        _rotation.MaxValue = 360f;
        _rotation.MinValue = 0f;

        _rotation.ValueChanged  += OnRotationChanged;
        _deleteButton.Pressed   += OnDeletePressed;
        _moveUpButton.Pressed   += OnMoveUpPressed;
        _moveDownButton.Pressed += OnMoveDownPressed;

        _typeOption.NewTypeRequested += OnNewTypeRequested;
    }

    private void OnNewTypeRequested(CrosshairShapeData shape)
    {
        if (_shape == shape)
            return;

        if (_shape != null)
            _crosshair.RemoveShape(_shape);
        
        _shape = shape;

        if (_shape != null)
            _crosshair.AddShape(_shape);

        SetShape(shape);
    }

    public void SetData(CrosshairData crosshair, CrosshairShapeData shape)
    {
        SetCrosshair(crosshair);
        SetShape(shape);
    }

    private void SetShape(CrosshairShapeData shape)
    {
        _shape = shape;

        _rotation.Value = _shape.RotationDegrees;

        _fill.SetData(_shape.FillData);
        _outlines.SetOutlineData(_shape.OutlineData);

        Clear();

        Control shapeSettings = CreateShapeSettings(shape);
        _shapeSettingsContainer.AddChild(shapeSettings);

        _typeOption.Initialize(shape);
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
        UI_CrosshairCircle circle = _circleScene.Instantiate<UI_CrosshairCircle>();
        circle.SetData(data);
        return circle;
    }

    private UI_CrosshairSquare CreateSquareSettings(CrosshairSquareData data)
    {
        UI_CrosshairSquare square = _squareScene.Instantiate<UI_CrosshairSquare>();
        square.SetData(data);
        return square;
    }

    private void OnRotationChanged(double value) =>
        _shape.RotationDegrees = (float) value;

    private void OnDeletePressed()
    {
        _crosshair.PropertyChanged -= UpdateVisibility;
        _crosshair.RemoveShape(_shape);
        QueueFree();
    }

    private void OnMoveDownPressed() =>
        _crosshair.MoveLayerDown(_shape);

    private void OnMoveUpPressed() =>
        _crosshair.MoveLayerUp(_shape);

    private void Clear()
    {
        foreach (Node child in _shapeSettingsContainer.GetChildren())
            child.QueueFree();
    }
}