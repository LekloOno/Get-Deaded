using System;

public interface PI_CrouchDerived
{
    protected bool IsActive {set;}
    public PI_Sprint SprintInput {get;}
    public EventHandler OnStartInput {get;}
    public EventHandler OnStopInput {get;}

    public abstract void InputStart();
    public abstract void InputStop();

    public void StartAction()
    {
        IsActive = true;
        OnStartInput?.Invoke(this, EventArgs.Empty);
        SprintInput.Reset();
    }

    public void StopAction()
    {
        IsActive = false;
        OnStopInput?.Invoke(this, EventArgs.Empty);
    }
}