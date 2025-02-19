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
        _capsule.Height = Mathf.Lerp(_capsule.Height, _colliderTargetScale, _scaleSpeed);

        Vector3 modelScale = ModelAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, _scaleSpeed);
        ModelAnchor.Scale = modelScale;
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