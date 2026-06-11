using System.Collections.Generic;
using System.IO;
using Godot;

[GlobalClass]
public partial class UI_CrosshairGalery : Control
{
    const string TitleSave      = "CROSSHAIR_BROWSE_SAVE";
    const string TitleCustom    = "CROSSHAIR_BROWSE_CUSTOM";
    const string TitlePresets   = "CROSSHAIR_BROWSE_PRESETS";

    const string FileEditSave   = "CROSSHAIR_FILE_EDIT_SAVE_PLACEHOLDER";
    const string FileEditSearch = "CROSSHAIR_FILE_EDIT_SEARCH_PLACEHOLDER";

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
        _fileEdit.TextChanged   += FileEditTextChanged;
        _confirmButton.Pressed  += ConfirmButtonPressed;
        _exportButton.Pressed   += ExportButtonPressed;
        _importButton.Pressed   += ImportButtonPressed;

        _exportDialog.FileSelected  += OnExportFileSelected;
        _importDialog.FilesSelected += OnImportFilesSelected;

        _exportDialog.MinSize = _importDialog.MinSize = new Vector2I(780, 580);
        _exportDialog.Theme   = _importDialog.Theme   = ThemeDB.GetDefaultTheme();
    }

    public void Init(List<CrosshairData> crosshairs, Mode mode)
    {
        _dataNameMap.Clear();

        SetMode(mode);

        _fileEdit.Text = "";

        foreach (Node node in _container.GetChildren())
            node.QueueFree();

        foreach (CrosshairData crosshair in crosshairs)
        {
            UI_CrosshairPreviewContainer preview = _crosshairStaticPreview.Instantiate<UI_CrosshairPreviewContainer>();
            preview.Init(crosshair, mode == Mode.BrowseCustom);
            preview.Selected    += OnCrosshairSelected;
            preview.Unselected  += OnCrosshairUnselected;
            
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

    private string ModeToTitle(Mode mode)
    {
        return mode switch
        {
            Mode.BrowsePresets  => TitlePresets,
            Mode.BrowseCustom   => TitleCustom,
            Mode.Save           => TitleSave,
            _ => throw new System.NotImplementedException(),
        };
    }

    private string ModeToEditPlaceHolder(Mode mode)
    {
        return mode switch
        {
            Mode.BrowsePresets  => FileEditSearch,
            Mode.BrowseCustom   => FileEditSearch,
            Mode.Save           => FileEditSave,
            _ => throw new System.NotImplementedException(),
        };
    }

    private void SetMode(Mode mode)
    {
        if (mode != Mode.BrowseCustom)
        {
            _exportButton.Disabled
            = _importButton.Disabled
            = true;
        }
        else
        {
            _exportButton.Disabled = _selectedData == null;
            _importButton.Disabled = false;
        }



        _confirmButton.Disabled = mode == Mode.Save
            && _fileEdit.Text.StripEdges() == string.Empty;
        
        _currentMode = mode;

        _titleLabel.Text = ModeToTitle(mode);
        _fileEdit.PlaceholderText = ModeToEditPlaceHolder(mode);
    }

    private void ImportButtonPressed()
    {
        if (_currentMode != Mode.BrowseCustom)
            return;
        
        _importDialog.PopupCentered();
    }

    private void ExportButtonPressed()
    {
        if (_currentMode != Mode.BrowseCustom)
            return;

        if (_selectedData == null)
            return;

        _exportDialog.CurrentFile = _fileEdit.Text + ".tres";
        _exportDialog.PopupCentered(); 
    }

    private void ConfirmButtonPressed()
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

    private void FileEditTextChanged(string search)
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

        _confirmButton.Disabled = search.StripEdges() == string.Empty;

        BaseButton button = _previewSelectGroup.GetPressedButton();
        if (button != null)
            button.ButtonPressed = false;
    }

    private void OnCrosshairSelected(CrosshairData data) =>
        SetSelectedData(data);

    private void OnCrosshairUnselected() => SetSelectedData(null);

    private void SetSelectedData(CrosshairData? data)
    {
        _selectedData = data;

        bool selected = _selectedData != null;
        _exportButton.Disabled = _currentMode != Mode.BrowseCustom || !selected;

        if (selected)
        {
            _fileEdit.Text = _selectedData!.ResourcePath.GetFile().GetBaseName();
            _fileEdit.CaretColumn = _fileEdit.Text.Length;
        }

        if (_currentMode == Mode.Save)
            _confirmButton.Disabled = _fileEdit.Text.StripEdges() == string.Empty;
        else
            _confirmButton.Disabled = !selected;
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