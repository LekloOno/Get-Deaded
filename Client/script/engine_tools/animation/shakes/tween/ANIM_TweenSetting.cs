using System;
using Godot;

[GlobalClass]
public partial class ANIM_TweenSetting : Resource
{
    [Export] public float AnimationTime {get; private set;}
    [Export] public Tween.TransitionType TransitionType {get; private set;}
    [Export] public Tween.EaseType EaseType {get; private set;}
    [Export] public string? PropertyPath {get; private set;}
    [Export] public ANIM_TweenValue? Value {get; private set;}

    public PropertyTweener? TweenProperty(Tween tween, GodotObject target, Variant? value = null)
    {
        tween.SetTrans(TransitionType);
        tween.SetEase(EaseType);

        Variant effectiveValue;
        if (value is Variant givenValue)
            effectiveValue = givenValue;
        else
        {
            if (Value != null)
                effectiveValue = Value.Value;
            else
            {
                GD.PrintErr("[ANIM_TweenSetting] missing Value.");
                return null;
            }            
        }

        if (PropertyPath == null)
        {
            GD.PrintErr("[ANIM_TweenSetting] missing PropertyPath.");
            return null;
        }

        return tween.TweenProperty(
            target,
            PropertyPath,
            effectiveValue,
            AnimationTime
        );
    }
}