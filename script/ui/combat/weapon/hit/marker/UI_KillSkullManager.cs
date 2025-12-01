using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_KillSkullManager : Control
{
    [Export] private UI_KillSkull _killTemplate;
    [Export] private UI_KillSkull _headshotKillTemplate;
    [Export] private float _fadeTime;

    public Action PushSkull;
    public Timer FadeTimer;

    public override void _Ready()
    {
        FadeTimer = new();
        AddChild(FadeTimer);

        if (_killTemplate.GetParent() is Node parent)
            parent.RemoveChild(_killTemplate);
            
        if (_headshotKillTemplate.GetParent() is Node headParent)
            headParent.RemoveChild(_headshotKillTemplate);
    }

    public void PopSkull(GC_BodyPart bodyPart, bool overrideBodyPart)
    {
        UI_KillSkull newSkull =  !overrideBodyPart && bodyPart == GC_BodyPart.Head
            ? (UI_KillSkull) _headshotKillTemplate.Duplicate()
            : (UI_KillSkull) _killTemplate.Duplicate();
        
        FadeTimer.Start(_fadeTime);
        FadeTimer.Timeout += newSkull.Fade;

        newSkull.MaxScaleReached += Push;
        newSkull.Init(this);

        AddChild(newSkull);
    }

    public void Push() => PushSkull?.Invoke();
}