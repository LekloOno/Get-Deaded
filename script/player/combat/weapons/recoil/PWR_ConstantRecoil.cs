using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PWR_ConstantRecoil : PW_Recoil
{
    [Export] private bool _autoReset;   // Automatically resets recoil when it's completed. Otherwise, the Fire is responsible for calling Reset().
    [Export] public Vector2 _angle;     // In degrees
    [Export] private float _time;       // Time to reach the recoil angle, in seconds
    [Export] private float _resetTime;  // Time to reset the recoil, in seconds
    private PC_Recoil _recoilController;

    public override void Initialize(PC_Recoil recoilController)
    {
        _recoilController = recoilController;
    }
    public override void Add() =>
        AddPartial(1);

    public override void AddPartial(double subHitSize)
    {
        PCR_BaseHandler handler = _recoilController.AddRecoil(_angle * Modifier.Result() * (float) subHitSize, _time);
        if (_autoReset)
            handler.Completed += Complete;
    }

    public override void Reset() => _recoilController.ResetRecoil(_resetTime);
    public override void ResetBuffer() => _recoilController.ResetBuffer();
    public override void Start()
    {
        _recoilController.ResetBuffer();
        Add();
    }

    private void Complete(object sender, EventArgs e) => _recoilController.ResetRecoil(_resetTime);
}