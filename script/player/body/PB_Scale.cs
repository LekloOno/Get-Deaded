using System;
using Godot;

[GlobalClass]
public partial class PB_Scale : CollisionShape3D
{
    [Export] private Node3D _modelAnchor;
    [Export] private PM_Controller _controller;
    [Export] private CollisionShape3D _bodyHitBox;

    public float ScaleDelta => _colliderInitScale - _capsule.Size.Y;
    public BoxShape3D Collider => _capsule; 

    private BoxShape3D _capsule;
    private CapsuleShape3D _bodyHitShape;

    private float _modelInitScale;
    private float _modelTargetScale;
    private float _colliderInitScale;
    private float _colliderTargetScale;
    private float _bodyHitBoxInitScale;
    private float _bodyHitBoxTargetScale;
    private float _scaleSpeed;

    private EventHandler OnPhysicsProcess;


    public override void _Ready()
    {
        _capsule = Shape as BoxShape3D;
        _bodyHitShape = _bodyHitBox.Shape as CapsuleShape3D;

        _colliderInitScale = _capsule.Size.Y;
        _modelInitScale = _modelAnchor.Scale.Y;
        _bodyHitBoxInitScale = _bodyHitShape.Height;
    }
    public override void _PhysicsProcess(double delta)
    {
        OnPhysicsProcess?.Invoke(this, EventArgs.Empty);
    }

    public void SetTargetScale(float targetScaleRatio, float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale * targetScaleRatio;
        _colliderTargetScale = _colliderInitScale * targetScaleRatio;
        _bodyHitBoxTargetScale = _bodyHitBoxInitScale * targetScaleRatio;

        _scaleSpeed = 1f - Mathf.Exp(-scaleSpeed*(float)GetPhysicsProcessDeltaTime()); // Magic trick to get frame rate independant lerping
        
        OnPhysicsProcess -= ProcessResetScale;
        OnPhysicsProcess -= ProcessScale;
        OnPhysicsProcess += ProcessScale;
    }

    public void ResetScale(float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale;
        _colliderTargetScale = _colliderInitScale;
        _bodyHitBoxTargetScale = _bodyHitBoxInitScale;

        _scaleSpeed = 1f - Mathf.Exp(-scaleSpeed*(float)GetPhysicsProcessDeltaTime()); // Magic trick to get frame rate independant lerping
        
        OnPhysicsProcess -= ProcessScale;
        OnPhysicsProcess -= ProcessResetScale;
        OnPhysicsProcess += ProcessResetScale;
    }

    public void ProcessResetScale(object sender, EventArgs e)
    {
        if (!PHX_Checks.CanUncrouch(_controller, this))
            return;
        
        ProcessScale(sender, e);

        if (ScaleDelta < 0.01f)
        {
            Vector3 size = _capsule.Size;
            size.Y = _colliderInitScale;
            _capsule.Size = size;

            Vector3 modelScale = _modelAnchor.Scale;
            modelScale.Y = _modelInitScale;
            _modelAnchor.Scale = modelScale;

            _bodyHitShape.Height = _bodyHitBoxInitScale;
            
            OnPhysicsProcess -= ProcessResetScale;
        }
    }

    public void ProcessScale(object sender, EventArgs e)
    {
        Vector3 size = _capsule.Size;
        size.Y = Mathf.Lerp(_capsule.Size.Y, _colliderTargetScale, _scaleSpeed);
        _capsule.Size = size;

        Vector3 modelScale = _modelAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, _scaleSpeed);
        _modelAnchor.Scale = modelScale;

        _bodyHitShape.Height = Mathf.Lerp(_bodyHitShape.Height, _bodyHitBoxTargetScale, _scaleSpeed);
    }
}