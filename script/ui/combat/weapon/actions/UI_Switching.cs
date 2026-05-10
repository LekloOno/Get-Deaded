using System;
using Godot;

[GlobalClass]
public partial class UI_Switching : TextureProgressBar
{
    [Export] private PW_WeaponsHandler _weaponsHandler;

    private float _switchTime = 0f;
    private float _elapsedTime = 0f;

    private Func<float> ComputeValue;

    public override void _Ready()
    {
        Visible = false;
        SetProcess(false);
        _weaponsHandler.SwitchIn += OnSwitchIn;
        _weaponsHandler.SwitchOut += OnSwitchOut;
        _weaponsHandler.SwitchCanceled += (_, _) => OnSwitchCancelled();
        _weaponsHandler.SwitchEnded += (_, _) => OnSwitchCancelled();
    }

    private void OnSwitchCancelled()
    {
        Visible = false;
        SetProcess(false);
    }

    private void OnSwitchOut(object sender, PW_Weapon e)
    {
        Visible = true;
        TintProgress = e.IconColor;
        _elapsedTime = 0f;
        _switchTime = e.SwitchOutTime;
        ComputeValue = () => ComputeValueOut;
        SetProcess(true);
    }

    private float ComputeValueOut =>
        1f - (_elapsedTime / _switchTime);

    private float ComputeValueIn =>
        _elapsedTime / _switchTime;

    private void OnSwitchIn(object sender, PW_Weapon e)
    {
        TintProgress = e.IconColor;
        _elapsedTime = 0f;
        _switchTime = e.SwitchInTime;
        ComputeValue = () => ComputeValueIn;
    }

    public override void _Process(double delta)
    {
        _elapsedTime += (float) delta;
        Value = ComputeValue();

        if (_elapsedTime >= _switchTime)
            OnSwitchCancelled();
    }
}