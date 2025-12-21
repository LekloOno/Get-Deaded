using Godot;
using System;

[GlobalClass]
public partial class PA_Looper : AudioStreamPlayer2D
{
    /// <summary>
    /// Time for the volume to reach _volume on start - in seconds.
    /// </summary>
    [Export] private float _fadeInTime;

    /// <summary>
    /// Time for the volume to reach 0 on stop - in seconds.
    /// </summary>
    [Export] private float _fadeOutTime;

    /// <summary>
    /// Max target volume - in DB.
    /// </summary>
    [Export] private float _volume;

    private Action OnUpdate;    // Allows to bind dynamically the fade in/out delegates on update and avoid a branching
    
    private float _fadeStart;   // Stores the time at which the fade started.
    private float _startVolume; // Stores the db volume at which a new fade operation has started.

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        VolumeDb = -80;
    }

    /// <summary>
    /// Starts Fading in.
    /// </summary>
    public void StartLoop()
    {
        InitFade();
        OnUpdate += FadeIn;
    }

    /// <summary>
    /// Starts Fading out.
    /// </summary>
    public void StopLoop()
    {
        InitFade();
        OnUpdate += FadeOut;
    }

    private void InitFade()
    {
        _fadeStart = PHX_Time.ScaledTicksMsec;
        _startVolume = VolumeDb;
        SetPhysicsProcess(true);
    }


    public override void _PhysicsProcess(double delta) =>
        OnUpdate?.Invoke();

    private void Fade(Action action, float currentFadeTime, float currentTargetVolume)
    {
        float elapsed = (PHX_Time.ScaledTicksMsec - _fadeStart)/1000f;

        if (elapsed >= currentFadeTime)
        {
            VolumeDb = currentTargetVolume;
            OnUpdate -= action;
            SetPhysicsProcess(false);
            return;
        }

        float elapsedScaled = elapsed/currentFadeTime;
        float lerped = MATH_Sound.LerpDB(_startVolume, currentTargetVolume, elapsedScaled);
        
        VolumeDb = lerped;
    }

    private void FadeOut() => Fade(FadeOut, _fadeOutTime, -80);
    private void FadeIn() => Fade(FadeIn, _fadeInTime, _volume);
}