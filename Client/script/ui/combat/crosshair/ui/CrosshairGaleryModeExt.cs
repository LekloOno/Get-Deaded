using Godot;
using static UI_CrosshairGalery;

public static class CrosshairGaleryModeExt
{
    const string TitleSave      = "CROSSHAIR_BROWSE_SAVE";
    const string TitleCustom    = "CROSSHAIR_BROWSE_CUSTOM";
    const string TitlePresets   = "CROSSHAIR_BROWSE_PRESETS";
    public static string ToTitle(this Mode mode)
    {
        return mode switch
        {
            Mode.BrowsePresets  => TitlePresets,
            Mode.BrowseCustom   => TitleCustom,
            Mode.Save           => TitleSave,
            _ => throw new System.NotImplementedException(),
        };
    }

    const string FileEditSave   = "CROSSHAIR_FILE_EDIT_SAVE_PLACEHOLDER";
    const string FileEditSearch = "CROSSHAIR_FILE_EDIT_SEARCH_PLACEHOLDER";
    public static string ToEditPlaceHolder(this Mode mode)
    {
        return mode switch
        {
            Mode.BrowsePresets  => FileEditSearch,
            Mode.BrowseCustom   => FileEditSearch,
            Mode.Save           => FileEditSave,
            _ => throw new System.NotImplementedException(),
        };
    }

    public static bool ToConfirmButtonDisabled(this Mode mode, string fileEditText, CrosshairData? data)
    {
        if (mode == Mode.Save)
            return fileEditText.StripEdges() == string.Empty;

        return data == null;
    }

    public static bool ToExportButtonDisabled(this Mode mode, CrosshairData? data)
    {
        if (mode != Mode.BrowseCustom)
            return true;

        return data == null;
    }

    public static bool ToImportButtonDisabled(this Mode mode) =>
        mode != Mode.BrowseCustom;
}