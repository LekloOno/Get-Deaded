using Godot;

[GlobalClass]
public partial class PB_Scale : CollisionShape3D
{
    [Export] private Node3D _modelAnchor;

    public float ScaleDelta => _colliderInitScale - _capsule.Size.Y;
    public BoxShape3D Collider => _capsule; 

    private BoxShape3D _capsule;

    private float _colliderInitScale;
    private float _modelInitScale;
    private float _colliderTargetScale;
    private float _modelTargetScale;
    private float _scaleSpeed;


    public override void _Ready()
    {
        _capsule = Shape as BoxShape3D;
        _colliderInitScale = _capsule.Size.Y;
        _modelInitScale = _modelAnchor.Scale.Y;
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector3 size = _capsule.Size;
        size.Y = Mathf.Lerp(_capsule.Size.Y, _colliderTargetScale, _scaleSpeed);
        _capsule.Size = size;

        Vector3 modelScale = _modelAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, _scaleSpeed);
        _modelAnchor.Scale = modelScale;
    }

    public void SetTargetScale(float targetScaleRatio, float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale * targetScaleRatio;
        _colliderTargetScale = _colliderInitScale * targetScaleRatio;
        _scaleSpeed = 1f - Mathf.Exp(-scaleSpeed*(float)GetPhysicsProcessDeltaTime()); // Magic trick to get frame rate independant lerping
    }

    public void ResetScale(float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale;
        _colliderTargetScale = _colliderInitScale;
        _scaleSpeed = 1f - Mathf.Exp(-scaleSpeed*(float)GetPhysicsProcessDeltaTime()); // Magic trick to get frame rate independant lerping
    }
}