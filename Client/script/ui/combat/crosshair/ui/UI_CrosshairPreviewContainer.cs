using System;
using Godot;

[GlobalClass]
public partial class UI_CrosshairPreviewContainer : Container
{
    [Export] private CrosshairPreview _preview = null!;
    [Export] private Button           _selectButton = null!;
    [Export] private Label            _title = null!;

    public event Action<CrosshairData>? Selected;

    public override void _Ready()
    {
        _selectButton.Pressed += OnSelected;
    }

    public void Init(CrosshairData data)
    {
        _preview.Data = data;
        _title.Text = data.ResourcePath.GetFile().GetBaseName();
    }

    private void OnSelected()
    {
        CrosshairSetting.Instance.Save(_preview.Data);
        Selected?.Invoke(_preview.Data);
    }
}