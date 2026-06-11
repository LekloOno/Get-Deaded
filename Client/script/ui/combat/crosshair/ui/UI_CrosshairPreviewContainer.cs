using System;
using Godot;

[GlobalClass]
public partial class UI_CrosshairPreviewContainer : Container
{
    [Export] private CrosshairPreview   _preview = null!;
    [Export] private Button             _selectButton = null!;
    [Export] private Label              _title = null!;
    [Export] private Button             _deleteButton = null!;
    [Export] private ConfirmationDialog _confirmDeleteDialog = null!;

    public event Action<CrosshairData>? Selected;
    public event Action<CrosshairData>? ConfirmSelected;
    public event Action? Unselected;

    public override void _Ready()
    {
        _selectButton.Toggled += OnToggled;
        _selectButton.GuiInput += OnSelectGuiGuiInput;
        _deleteButton.Pressed += OnDeletePressed;
        _confirmDeleteDialog.Confirmed += DeleteCrosshair;
    }

    private void OnSelectGuiGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mb)
            return;

        if (mb.ButtonIndex == MouseButton.Left && mb.DoubleClick)
            ConfirmSelected?.Invoke(_preview.Data);
    }

    public void Init(CrosshairData data, bool custom = false)
    {
        _preview.Data = data;
        _title.Text = data.ResourcePath.GetFile().GetBaseName();
        _deleteButton.Visible = custom;
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

    private void DeleteCrosshair()
    {
        string path = _preview.Data.ResourcePath;
        var error = DirAccess.RemoveAbsolute(path);

        if (error != Error.Ok)
            GD.PrintErr($"Failed to delete {path}: {error}");
        else
        {
            GD.Print($"Deleted {path}");
            QueueFree();
        }
    }

    private void OnDeletePressed()
    {
        _confirmDeleteDialog.PopupCentered();
    }
}