using System;
using Godot;

/// <summary>
/// Handles timer for reload steps and keeps such steps in memory for cancel, restart, etc.
/// Does not actually perform the reloads, as they are specific to the architecture of the weapon
/// which might use multiple PW_Ammunitions, for different fires, etc.
/// </summary>
public partial class PW_WeaponReloader : Node
{
    const bool IgnoreTimeScale = true;

    public PW_WeaponReloader() {}

    public PW_WeaponReloader(PW_WeaponReloaderData data)
    {
        _data = data;
    }

    public PW_ReloadStep CurrentStep {get; private set;} = PW_ReloadStep.Ready;
    public Action<PW_ReloadStep, PW_ReloadStep, float>? Started;
    public Action<PW_ReloadStep, PW_ReloadStep>? Canceled;
    public Action? Unloaded;
    public Action? Inserted;
    public Action? Chambered;
    public Action? Recovered;

    private SceneTreeTimer? _timer;
    private PW_WeaponReloaderData _data;

    public bool IsReady => CurrentStep == PW_ReloadStep.Ready;

    public override void _Ready() => Reset();

    public void Reset()
    {
        SetPhysicsProcess(false);
        _acc = 0f;
    }

    public bool StartReload()
    {
        if (IsPhysicsProcessing())
            return false;

        PW_ReloadStep prev = CurrentStep;

        if (CurrentStep == PW_ReloadStep.Ready)
            CurrentStep = PW_ReloadStep.Unload;

        _actionLength = CurrentStep switch
        {
            PW_ReloadStep.Unload => _data.UnloadTime,
            PW_ReloadStep.Insert => _data.InsertTime,
            PW_ReloadStep.Chamber => _data.ChamberTime,
            _ => throw new NotImplementedException(), // Ready becomes unload, recover should be skipped by previous cancel
        };

        SetPhysicsProcess(true);

        Started?.Invoke(prev, CurrentStep, CurrentStep.ReloadTime(_data));
        return true;
    }

    public bool Cancel()
    {
        if (!IsPhysicsProcessing())
            return false;

        PW_ReloadStep prev = CurrentStep;

        if (CurrentStep == PW_ReloadStep.Recover)
        {
            CurrentStep = PW_ReloadStep.Ready;  // animation skip on recover
            Recovered?.Invoke();
        }
        
        Reset();

        Canceled?.Invoke(prev, CurrentStep);

        return true;
    }

    private double _acc = 0f;
    private double _actionLength;
    public override void _PhysicsProcess(double delta)
    {
        if (IgnoreTimeScale)
            _acc += delta;
        else
            _acc += delta * Engine.TimeScale;

        if (_acc < _actionLength)
            return;

        _acc -= _actionLength; // Keep in memory the potential delay

        _actionLength = CurrentStep switch
        {
            PW_ReloadStep.Unload => Unload(),
            PW_ReloadStep.Insert => Insert(),
            PW_ReloadStep.Chamber => Chamber(),
            _ => Recover(),
        };
    }

    private float Unload()
    {
        GD.Print("---- unload");
        CurrentStep = PW_ReloadStep.Insert;
        Unloaded?.Invoke();
        return _data.InsertTime;
    }

    private float Insert()
    {
        float time;

        if (_data.ChamberTime > 0f)
        {
            CurrentStep = PW_ReloadStep.Chamber;
            time = _data.ChamberTime;
        }
        else
        {
            CurrentStep = PW_ReloadStep.Recover;
            time = _data.RecoverTime;
        }

        Inserted?.Invoke();
        return time;
    }

    private float Chamber()
    {
        CurrentStep = PW_ReloadStep.Recover;
        Chambered?.Invoke();
        return _data.RecoverTime;
    }

    private float Recover()
    {
        CurrentStep = PW_ReloadStep.Ready;
        Recovered?.Invoke();
        Reset();
        return 0f;
    }
    
    public float ReloadTime()
    {
        float time = CurrentStep.ReloadTime(_data);

        if (_timer?.TimeLeft > 0f)
            time += (float)_timer.TimeLeft - CurrentStep.StepTime(_data);

        return time;
    }
}