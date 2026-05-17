using System;
using Godot;

[GlobalClass]
public partial class PWR_ConstantRecoil : PW_Recoil
{
    /// <summary>
    /// Automatically resets recoil when it's completed. Otherwise, the Fire is responsible for calling Reset().
    /// </summary>
    [Export] private bool _autoReset;
    /// <summary>
    /// Recoil angle in degrees.
    /// </summary>
    [Export] public Vector2 _angle;
    /// <summary>
    /// Time to reach the recoil angle, in seconds.
    /// </summary>
    [Export] private float _time;
    /// <summary>
    /// Time to reset the recoil, in seconds.
    /// </summary>
    [Export] private float _resetTime;
    /// <summary>
    /// Indicates if recoil is only applied at the start of recoil, or if continuous recoil is also applied. <br/>
    /// True by default.
    /// </summary>
    [Export] private bool _continuous = true;
    private PC_Recoil _recoilController;

    public override void Initialize(PC_Recoil recoilController)
    {
        _recoilController = recoilController;
    }
    public override void Add()
    {
        if (_continuous)
            AddPartial(1);
    }

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
        AddPartial(1);
    }

    private void Complete(object sender, EventArgs e) => _recoilController.ResetRecoil(_resetTime);
}