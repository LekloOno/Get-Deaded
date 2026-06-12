using System.Collections.Generic;
using System.IO;
using Godot;

[GlobalClass]
public partial class UI_CrosshairGalery : Control
{
    [Export] private UI_EscapeMenu _menu = null!;
    [Export] private Container     _container = null!;
    [Export] private PackedScene   _crosshairStaticPreview = null!;
    [Export] private Label         _titleLabel = null!;
    [Export] private Button        _closeButton = null!;
    [Export] private LineEdit      _fileEdit = null!;
    [Export] private Button        _confirmButton = null!;
    [Export] private Button        _exportButton = null!;
    [Export] private Button        _importButton = null!;
    [Export] private ButtonGroup   _previewSelectGroup = null!;
    [Export] private FileDialog    _exportDialog = null!;
    [Export] private FileDialog    _importDialog = null!;

    private readonly Dictionary<UI_CrosshairPreviewContainer, string> _dataNameMap = [];

    private Mode _currentMode;
    private CrosshairData? _selectedData;

    public override void _Ready()
    {
        _closeButton.Pressed    += _menu.ExitCurrent;
        _fileEdit.TextChanged   += OnFileEditTextChanged;
        _fileEdit.TextSubmitted += OnFileEditTextSubmitted;
        _confirmButton.Pressed  += OnConfirmButtonPressed;
        _exportButton.Pressed   += OnExportButtonPressed;
        _importButton.Pressed   += OnImportButtonPressed;

        _exportDialog.FileSelected  += OnExportFileSelected;
        _importDialog.FilesSelected += OnImportFilesSelected;

        _exportDialog.MinSize = _importDialog.MinSize = new Vector2I(780, 580);
        _exportDialog.Theme   = _importDialog.Theme   = ThemeDB.GetDefaultTheme();

        VisibilityChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged()
    {
        if (IsVisibleInTree())
            return;
            
        Clear();
    }

    public void Clear()
    {
        _dataNameMap.Clear();
        _fileEdit.Text = "";
        foreach (Node node in _container.GetChildren())
            node.QueueFree();

        _selectedData = null;
    }

    public void Init(List<CrosshairData> crosshairs, Mode mode)
    {
        Clear();
        SetMode(mode);

        foreach (CrosshairData crosshair in crosshairs)
        {
            UI_CrosshairPreviewContainer preview = _crosshairStaticPreview.Instantiate<UI_CrosshairPreviewContainer>();
            preview.Init(crosshair, mode == Mode.BrowseCustom);
            preview.Selected        += OnCrosshairSelected;
            preview.Unselected      += OnCrosshairUnselected;
            preview.ConfirmSelected += OnCrosshairConfirmSelected;
            
            _dataNameMap.Add(preview, crosshair.ResourcePath.GetFile().GetBaseName());
            _container.AddChild(preview);
        }
    }

    public enum Mode
    {
        /// <summary>
        /// - Use a crosshair among presets.
        /// </summary>
        BrowsePresets,
        /// <summary>
        /// - Use a crosshair among saved ones. <br/>
        /// - Export selected crosshair to file system. <br/>
        /// - Import crosshair from file system. <br/>
        /// </summary>
        BrowseCustom,
        /// <summary>
        /// - Save current crosshair to new custom preset.
        /// </summary>
        Save,
    }

    private void SetMode(Mode mode)
    {
        _importButton.Disabled = mode.ToImportButtonDisabled();
        _exportButton.Disabled = mode.ToExportButtonDisabled(_selectedData);

        _confirmButton.Disabled = mode
            .ToConfirmButtonDisabled(_fileEdit.Text, _selectedData);
        
        _currentMode = mode;

        _titleLabel.Text = mode.ToTitle();
        _fileEdit.PlaceholderText = mode.ToEditPlaceHolder();
    }

    private void OnImportButtonPressed()
    {
        if (_currentMode != Mode.BrowseCustom)
            return;
        
        _importDialog.PopupCentered();
    }

    private void OnExportButtonPressed()
    {
        if (_currentMode != Mode.BrowseCustom)
            return;

        if (_selectedData == null)
            return;

        _exportDialog.CurrentFile = _fileEdit.Text + ".tres";
        _exportDialog.PopupCentered(); 
    }

    private void OnConfirmButtonPressed()
    {
        if (_currentMode == Mode.Save)
            ConfirmSave();
        else if (_selectedData != null)
            ConfirmUse(_selectedData);
        else
            return;

        _menu.ExitCurrent();
    }

    private void ConfirmUse(CrosshairData data)
    {
        CrosshairSetting.Instance.Save(data);
    }

    private void ConfirmSave()
    {
        CrosshairSetting.SaveAs(CrosshairSetting.Instance.Data, _fileEdit.Text);
    }

    private void OnFileEditTextSubmitted(string newText) =>
        OnConfirmButtonPressed();

    private void OnFileEditTextChanged(string search)
    {
        bool found = false;

        foreach ((UI_CrosshairPreviewContainer preview, string name) in _dataNameMap)
        {
            if (!found && name.Equals(search))
            {
                found = true;
                preview.Visible = true;
                preview.SetSelected();
                continue;
            }
            
            preview.Visible = name.Contains(search, System.StringComparison.CurrentCultureIgnoreCase);
        }

        if (found || _currentMode != Mode.Save)
            return;

        _confirmButton.Disabled = _currentMode
            .ToConfirmButtonDisabled(search, _selectedData);

        BaseButton button = _previewSelectGroup.GetPressedButton();
        if (button != null)
            button.ButtonPressed = false;
    }

    private void OnCrosshairSelected(CrosshairData data) =>
        SetSelectedData(data);

    private void OnCrosshairUnselected() => SetSelectedData(null);

    private void OnCrosshairConfirmSelected(CrosshairData data)
    {
        _selectedData = data;
        OnConfirmButtonPressed();
    }

    private void SetSelectedData(CrosshairData? data)
    {
        _selectedData = data;

        _exportButton.Disabled = _currentMode
            .ToExportButtonDisabled(_selectedData);

        if (_selectedData != null)
        {
            _fileEdit.Text = _selectedData!.ResourcePath.GetFile().GetBaseName();
            _fileEdit.CaretColumn = _fileEdit.Text.Length;
        }

        _confirmButton.Disabled = _currentMode
            .ToConfirmButtonDisabled(_fileEdit.Text, _selectedData);
    }

    private void OnExportFileSelected(string path)
    {
        if (string.IsNullOrWhiteSpace(Path.GetExtension(path)))
            path += ".tres";

        var error = ResourceSaver.Save(_selectedData, path);

        if (error != Error.Ok)
            GD.PrintErr($"Failed to export crosshair at {path}: {error}");
        else
            GD.Print($"Exported to {path}");
    }

    private void OnImportFilesSelected(string[] paths)
    {
        foreach (var path in paths)
        {
            var crosshairData = ResourceLoader.Load<CrosshairData>(path);

            if (crosshairData != null)
            {
                GD.Print($"Imported crosshair: {path}");
                var name = Path.GetFileNameWithoutExtension(path);
                CrosshairSetting.SaveAs(crosshairData, name);
            }
            else
                GD.PrintErr($"Invalid crosshair file: {path}");
        }

        Init(CrosshairSetting.OpenSaved(), Mode.BrowseCustom);
    }
}