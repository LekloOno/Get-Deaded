using Godot;

[GlobalClass]
public partial class PB_Scale : CollisionShape3D
{
    [Export] public Node3D ModelAnchor {get; private set;}
    private CapsuleShape3D _capsule;

    private float _colliderInitScale;
    private float _modelInitScale;
    private float _colliderTargetScale;
    private float _modelTargetScale;
    private float _scaleSpeed;


    public override void _Ready()
    {
        _capsule = Shape as CapsuleShape3D;
        _colliderInitScale = _capsule.Height;
        _modelInitScale = ModelAnchor.Scale.Y;
    }
    public override void _PhysicsProcess(double delta)
    {
        float speed = _scaleSpeed*(float)delta;
        _capsule.Height = Mathf.Lerp(_capsule.Height, _colliderTargetScale, speed);

        Vector3 modelScale = ModelAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, speed);
        ModelAnchor.Scale = modelScale;
    }

    public void SetTargetScale(float targetScaleRatio, float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale * targetScaleRatio;
        _colliderTargetScale = _colliderInitScale * targetScaleRatio;
        _scaleSpeed = scaleSpeed;
    }

    public void ResetScale(float scaleSpeed)
    {
        _modelTargetScale = _modelInitScale;
        _colliderTargetScale = _colliderInitScale;
        _scaleSpeed = scaleSpeed;
    }
}