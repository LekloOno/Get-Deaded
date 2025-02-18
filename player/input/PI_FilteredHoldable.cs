public interface PI_FilteredHoldable
{
    bool IsActive {get;}
    bool Hold {get;}
    
    protected void StartAction();
    protected void StopAction();

    public void Handle(bool down)
    {
        if (Hold)
            HandleHold(down);
        else
            HandleSimple(down);
    }

    public void HandleHold(bool down)
    {
        if (down)
            StartAction();
        else
            StopAction();
    }

    public void HandleSimple(bool down)
    {
        if (down)
        {
            if (IsActive)
                StopAction();
            else
                StartAction();
        }
    }
}