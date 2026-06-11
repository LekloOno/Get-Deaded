using System.Collections.Generic;
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
            preview.Init(crosshair);
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
        _exportButton.Disabled
        = _importButton.Disabled
        = mode != Mode.BrowseCustom;



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
        
        // TODO - open file dialog to import
    }

    private void ExportButtonPressed()
    {
        if (_currentMode != Mode.BrowseCustom)
            return;
        
        // TODO - open file dialog to export 
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

        if (_selectedData != null)
            _fileEdit.Text = _selectedData.ResourcePath.GetFile().GetBaseName();

        if (_currentMode == Mode.Save)
            _confirmButton.Disabled = _fileEdit.Text.StripEdges() == string.Empty;
        else
            _confirmButton.Disabled = _selectedData == null;
    }
}