using System;
using Godot;

[GlobalClass]
public partial class UI_CrosshairPreviewContainer : Container
{
    [Export] private CrosshairPreview _preview = null!;
    [Export] private Button           _selectButton = null!;
    [Export] private Label            _title = null!;

    public event Action<CrosshairData>? Selected;
    public event Action? Unselected;

    public override void _Ready()
    {
        _selectButton.Toggled += OnToggled;
    }

    public void Init(CrosshairData data)
    {
        _preview.Data = data;
        _title.Text = data.ResourcePath.GetFile().GetBaseName();
    }

    public void SetSelected() =>
        _selectButton.ButtonPressed = true;

    private void OnToggled(bool pressed)
    {
        if (pressed)
            Selected?.Invoke(_preview.Data);
        else if (_selectButton.ButtonGroup.GetPressedButton() == null)
            Unselected?.Invoke();
    }
}