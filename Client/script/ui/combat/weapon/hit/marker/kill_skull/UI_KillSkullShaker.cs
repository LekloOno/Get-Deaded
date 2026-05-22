using System;
using Godot;

[GlobalClass]
public partial class UI_KillSkullShaker : Control
{
    [Export] private UI_KillSkull _killSkull; 
    [Export] public ANIM_Vec2TraumaLayer ShakeLayer;
    [Export] public float Trauma = 1f;
    [Export] public float ShakeIntensity = 10f;
    public Texture2D Texture
    {
        get => _killSkull.Texture;
        set => _killSkull.Texture = value;
    }

    public Action MaxScaleReached
    {
        get => _killSkull.MaxScaleReached;
        set => _killSkull.MaxScaleReached = value;
    }

    private double _time;
    private bool _critical;

    public void Init(UI_KillSkullManager manager, bool critical = false)
    {
        _critical = critical;

        _killSkull.Removed += OnSkullRemoved;
        _killSkull.Init(manager);

        ShakeLayer.AddTrauma(Trauma);

        if (_critical)
            EnemiesColorSetting.Instance.Changed += OnChanged;
    }

    private void OnSkullRemoved()
    {
        if (_critical)
            EnemiesColorSetting.Instance.Changed -= OnChanged;

        QueueFree();
    }


    private void OnChanged(GodotObject sender, Variant value)
    {
        Modulate = EnemiesColorSetting.Color;
    }


    public override void _Process(double delta)
    {
        _time += delta;
        Position = ShakeLayer.GetShakeAngleIntensity((float) delta, (float) _time) * ShakeIntensity;
    }
}