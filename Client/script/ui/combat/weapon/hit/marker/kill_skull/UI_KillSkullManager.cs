using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_KillSkullManager : Control
{
    [Export] private PackedScene _killTemplate;
    [Export] private Texture2D _normalTexture;
    [Export] private Texture2D _criticalTexture;
    [Export] private float _fadeTime;
    
    // Live edit - should later be removed, it's purely to live-tweak the values of _killTemplate
    [Export] private float _trauma = 1f;
    [Export] private float _shakeIntensity = 10f;
    [Export] private ANIM_Vec2TraumaLayer _shakeLayer;
    [Export] public uint MaxChainSize {get; private set;} = 8;


    public Action PushSkull;
    public Timer FadeTimer;

    public override void _Ready()
    {
        FadeTimer = new()
        {
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };
        
        AddChild(FadeTimer);
    }

    public void PopSkull(bool critical)
    {
        UI_KillSkullShaker newSkull = (UI_KillSkullShaker) _killTemplate.Instantiate();
        newSkull.ShakeIntensity = _shakeIntensity;
        newSkull.Trauma = _trauma;
        newSkull.ShakeLayer = (ANIM_Vec2TraumaLayer) _shakeLayer.Duplicate();
        
        if (critical)
        {
            newSkull.Texture = _criticalTexture;
            newSkull.Modulate = CONF_HitColors.Colors.Critical;
        } else
        {
            newSkull.Texture = _normalTexture;
            newSkull.Modulate = CONF_HitColors.Colors.Normal;
        }

        FadeTimer.Start(_fadeTime);
        newSkull.MaxScaleReached += Push;

        AddChild(newSkull);
        newSkull.Init(this);
    }

    public void Push() => PushSkull?.Invoke();
}