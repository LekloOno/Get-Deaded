using System;
using Godot;

[GlobalClass]
public partial class PW_ConstantRecoil : PW_Recoil
{
    [Export] private bool _autoReset;
    [Export] public Vector2 _angle;
    [Export] private float _time;
    [Export] private float _resetTime;
    private PC_Recoil _recoilController;

    public override void Initialize(PC_Recoil recoilController)
    {
        _recoilController = recoilController;
    }
    public override void Add()
    {
        PC_BaseHandler handler = _recoilController.AddRecoil(_angle, _time);
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