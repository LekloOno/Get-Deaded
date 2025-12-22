using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PHX_BodyScale : CollisionShape3D
{
    [Export] private Node3D _spatialAnchor;
    [Export] private PhysicsBody3D _physicsBody;
    [Export] private CollisionShape3D _bodyHitBox;

    public float ScaleDelta => _colliderInitScale - _box.Size.Y;
    public BoxShape3D Collider => _box; 

    private BoxShape3D _box;
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
        _box = Shape as BoxShape3D;
        _bodyHitShape = _bodyHitBox.Shape as CapsuleShape3D;

        _colliderInitScale = _box.Size.Y;
        _modelInitScale = _spatialAnchor.Scale.Y;
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

    private void ProcessResetScale(object sender, EventArgs e)
    {
        if (!PHX_Checks.CanUncrouch(_physicsBody, this))
            return;
        
        ProcessScale(sender, e);

        if (ScaleDelta < 0.01f)
        {
            Vector3 size = _box.Size;
            size.Y = _colliderInitScale;
            _box.Size = size;

            Vector3 modelScale = _spatialAnchor.Scale;
            modelScale.Y = _modelInitScale;
            _spatialAnchor.Scale = modelScale;

            _bodyHitShape.Height = _bodyHitBoxInitScale;
            
            OnPhysicsProcess -= ProcessResetScale;
        }
    }

    private void ProcessScale(object sender, EventArgs e)
    {
        Vector3 size = _box.Size;
        size.Y = Mathf.Lerp(_box.Size.Y, _colliderTargetScale, _scaleSpeed);
        _box.Size = size;

        Vector3 modelScale = _spatialAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, _scaleSpeed);
        _spatialAnchor.Scale = modelScale;

        _bodyHitShape.Height = Mathf.Lerp(_bodyHitShape.Height, _bodyHitBoxTargetScale, _scaleSpeed);
    }
}