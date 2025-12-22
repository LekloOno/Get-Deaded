using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class UIW_FireHolder : BoxContainer
{
    protected PW_Ammunition _ammos;

    public void Initialize(PW_Fire fire)
    {
        if (_ammos != null)
        {
            _ammos.LoadedChanged -= HandleLoaded;
            _ammos.UnloadedChanged -= HanldedUnloaded;
        }

        _ammos = fire.Ammos;
        _ammos.LoadedChanged += HandleLoaded;
        _ammos.UnloadedChanged += HanldedUnloaded;

        InnerInitialize(fire);
    }

    public abstract void InnerInitialize(PW_Fire fire);
    protected abstract void HandleLoaded(int amount, uint finalAmount);
    protected abstract void HanldedUnloaded(int amount, uint finalAmount);
}