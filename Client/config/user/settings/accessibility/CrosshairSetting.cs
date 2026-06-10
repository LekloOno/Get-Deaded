using System;
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

        EnsureSavedDirectoryExists();

        ResourceSaver.Save(Data, UserCrosshairPath);

        DataSwapped?.Invoke(prev, Data);
    }

    private static void EnsureSavedDirectoryExists()
    {
        if (DirAccess.DirExistsAbsolute(UserCrosshairDir))
            return;

        var error = DirAccess.MakeDirRecursiveAbsolute(UserCrosshairDir);

        if (error != Error.Ok)
            GD.PushError($"Failed to create directory '{UserCrosshairDir}': {error}");
    }
}