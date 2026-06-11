using System;
using System.Collections.Generic;
using Godot;

public partial class CrosshairSetting : Node
{
    public const string UserCrosshairDir = "user://crosshair";
    public const string UserCrosshair = "crosshair.tres";
    public const string UserCrosshairPath = UserCrosshairDir + "/" + UserCrosshair;
    public const string DefaultCrosshairPath = "res://default_crosshair.tres";
    public static CrosshairSetting Instance = null!;
    public CrosshairData Data {get; private set;} = null!;


    public event Action<CrosshairData>? DataInitialized;
    public event Action<CrosshairData, CrosshairData>? DataSwapped;

    public override void _Ready()
    {
        Instance = this;

        Data = ResourceLoader.Exists(UserCrosshairPath)
            ? ResourceLoader.Load<CrosshairData>(UserCrosshairPath)
            : ResourceLoader.Load<CrosshairData>(DefaultCrosshairPath);

        DataInitialized?.Invoke(Data);
    }

    public void Save(CrosshairData data)
    {
        CrosshairData prev = Data;
        Data = (CrosshairData) data.Duplicate(true);

        EnsureDirectoriesExists();

        ResourceSaver.Save(Data, UserCrosshairPath);

        DataSwapped?.Invoke(prev, Data);
    }

    private static void EnsureDirectoriesExists()
    {
        if (DirAccess.DirExistsAbsolute(UserSavedCrosshairDirPath))
            return;
        
        var error = DirAccess.MakeDirRecursiveAbsolute(UserSavedCrosshairDirPath);

        if (error != Error.Ok)
            GD.PushError($"Failed to create directory '{UserCrosshairDir}': {error}");
    }

    const string CrosshairPresetsJsonRegistry = "res://config/assets/crosshair/registry.json";
    const string UserSavedCrosshairDir = "saved";
    const string UserSavedCrosshairDirPath = UserCrosshairDir + "/" + UserSavedCrosshairDir;

    public static CrosshairData SaveAs(CrosshairData data, string name)
    {
        CrosshairData cachedData = (CrosshairData) data.Duplicate(true);
        EnsureDirectoriesExists();
        string filePath = UserSavedCrosshairDirPath + "/" + name + ".tres";
        var error = ResourceSaver.Save(cachedData, filePath);

        if (error != Error.Ok)
            GD.PushError($"Failed to save crosshair '{filePath}': {error}");

        return cachedData;
    }

    public static List<CrosshairData> OpenPresets()
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

            var crosshair = ResourceLoader.Load<CrosshairData>(path);

            if (crosshair == null)
            {
                GD.PushWarning($"Failed to load weapon: {path}");
                continue;
            }

            crosshairs.Add(crosshair);
        }

        return crosshairs;
    }

    public static List<CrosshairData> OpenSaved()
    {
        EnsureDirectoriesExists();
        
        List<CrosshairData> crosshairs = [];

        if (!DirAccess.DirExistsAbsolute(UserSavedCrosshairDirPath))
        {
            GD.PushWarning($"Saved crosshair directory not found: {UserSavedCrosshairDirPath}");
            return crosshairs;
        }

        using var dir = DirAccess.Open(UserSavedCrosshairDirPath);

        if (dir == null)
        {
            GD.PushError($"Failed to open directory: {UserSavedCrosshairDirPath}");
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

            string path = $"{UserSavedCrosshairDirPath}/{fileName}";

            var resource = ResourceLoader.Load<CrosshairData>(path, cacheMode: ResourceLoader.CacheMode.IgnoreDeep);

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