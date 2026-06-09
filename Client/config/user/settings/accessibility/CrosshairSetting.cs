using System;
using Godot;

public partial class CrosshairSetting : Node
{
    public const string UserCrosshairPath = "user://crosshair.tres";
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
        Data = data;

        ResourceSaver.Save(Data, UserCrosshairPath);
        GD.Print("saved!");
        DataSwapped?.Invoke(prev, Data);
    }
}