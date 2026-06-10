using System.Collections.Generic;
using Godot;

public partial class UI_CrosshairPresets : Node
{
    [Export] private UI_EscapeMenu      _menu   = null!;
    [Export] private UI_CrosshairGalery _galery = null!;
    [Export] private Button? _presetsButton;
    [Export] private Button? _savedButton;

    const string CrosshairPresetsJsonRegistry = "res://config/assets/crosshair/registry.json";
    const string CrosshairSavedDir = "user://crosshair/saved";

    public override void _Ready()
    {
        if (_presetsButton != null)
            _presetsButton.Pressed += OnPresetsPressed;
            
        if (_savedButton != null)
            _savedButton.Pressed += OnSavedPressed;
    }

    private void OnPresetsPressed()
    {
        _galery.Init(OpenPresets());
        _menu.Enter(_galery);
    }

    private void OnSavedPressed()
    {
        _galery.Init(OpenSaved());
        _menu.Enter(_galery);
    }

    private List<CrosshairData> OpenPresets()
    {
        List<CrosshairData> crosshairs = [];

        if (!FileAccess.FileExists(CrosshairPresetsJsonRegistry))
        {
            GD.PushError($"Crosshair registry not found: {CrosshairPresetsJsonRegistry}");
            return crosshairs;
        }

        using var file = FileAccess.Open(CrosshairPresetsJsonRegistry, FileAccess.ModeFlags.Read);
        var jsonText = file.GetAsText();

        var parsed = Json.ParseString(jsonText);

        if (parsed.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError("Invalid JSON format: expected dictionary root.");
            return crosshairs;
        }

        var root = parsed.AsGodotDictionary();

        if (!root.ContainsKey("crosshairs"))
        {
            GD.PushError("Invalid JSON: missing 'crosshairs' key.");
            return crosshairs;
        }

        var weaponsArray = root["crosshairs"].AsGodotArray();

        foreach (var entry in weaponsArray)
        {
            string path = entry.AsString();

            if (string.IsNullOrEmpty(path))
                continue;

            var crosshair = GD.Load<CrosshairData>(path);

            if (crosshair == null)
            {
                GD.PushWarning($"Failed to load weapon: {path}");
                continue;
            }

            crosshairs.Add(crosshair);
        }

        return crosshairs;
    }

    private List<CrosshairData> OpenSaved()
    {
        List<CrosshairData> crosshairs = [];

        if (!DirAccess.DirExistsAbsolute(CrosshairSavedDir))
        {
            GD.PushWarning($"Saved crosshair directory not found: {CrosshairSavedDir}");
            return crosshairs;
        }

        using var dir = DirAccess.Open(CrosshairSavedDir);

        if (dir == null)
        {
            GD.PushError($"Failed to open directory: {CrosshairSavedDir}");
            return crosshairs;
        }

        dir.ListDirBegin();

        while (true)
        {
            string fileName = dir.GetNext();

            if (string.IsNullOrEmpty(fileName))
                break;

            if (dir.CurrentIsDir())
                continue;

            string extension = fileName.GetExtension().ToLower();

            if (extension != "tres" && extension != "res")
                continue;

            string path = $"{CrosshairSavedDir}/{fileName}";

            var resource = ResourceLoader.Load<CrosshairData>(path);

            if (resource == null)
            {
                GD.PushWarning($"Failed to load crosshair: {path}");
                continue;
            }

            crosshairs.Add(resource);
        }

        dir.ListDirEnd();

        return crosshairs;
    }
}