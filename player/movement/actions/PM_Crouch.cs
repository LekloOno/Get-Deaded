using System;
using Godot;

[GlobalClass]
public partial class PM_Crouch : PM_Action
{
    [Export] public PI_Crouch CrouchInput {get; private set;}
    [Export] public PM_Jump Jump {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public CollisionShape3D CollisionShape {get; private set;}
    [Export] public Node3D ModelAnchor {get; private set;}
    [Export] public float SlideMinSpeed;
    [Export] public float ScaleSpeed;
    [Export(PropertyHint.Range, "0.2,1.0")] public float TargetScaleRatio {get; private set;}
    private CapsuleShape3D _capsule;
    //[Export] public float 
    private bool _canDash;
    private float _colliderInitScale;
    private float _modelInitScale;
    private float _colliderCrouchScale;
    private float _modelCrouchScale;
    private float _colliderTargetScale;
    private float _modelTargetScale;

    public EventHandler OnStartCrouch;
    public EventHandler OnStopCrouch;

    private EventHandler OnCrouchUpdate;

    public override void _Ready()
    {
        _capsule = CollisionShape.Shape as CapsuleShape3D;

        _modelInitScale = ModelAnchor.Scale.Y;
        _colliderInitScale = _capsule.Height;

        _modelCrouchScale = _modelInitScale * TargetScaleRatio;
        _colliderCrouchScale = _colliderInitScale * TargetScaleRatio;

        _modelTargetScale = _modelInitScale;
        _colliderTargetScale = _colliderInitScale;

        CrouchInput.OnStartInput += (o, e) => StartCrouch();
        CrouchInput.OnStopInput += (o, e) => StopCrouch();
    }

    public void CrouchUpdate()
    {
        OnCrouchUpdate?.Invoke(this, EventArgs.Empty);
        // OnStartCrouch
        //      grounded
        //          no speed - start crouch
    //              speed - start slide
        //      airborne
        //          start slide with no boost
        //          _canDash - start dash

        // OnStopCrouch
        //      grounded
        //          start normal
        //      airborne
        //          stop slide

        // OnLanding
        //      dashing - stop dashing
        //
        // SpeedTooLow and grounded
        //      stop sliding
    }

    public override void _PhysicsProcess(double delta)
    {
        float speed = ScaleSpeed*(float)delta;
        
        _capsule.Height = Mathf.Lerp(_capsule.Height, _colliderTargetScale, speed);

        Vector3 modelScale = ModelAnchor.Scale;
        modelScale.Y = Mathf.Lerp(modelScale.Y, _modelTargetScale, speed);
        ModelAnchor.Scale = modelScale;
    }

    public void StartCrouch()
    {
        _modelTargetScale = _modelCrouchScale;
        _colliderTargetScale = _colliderCrouchScale;
        OnStartCrouch?.Invoke(this, EventArgs.Empty);
        GD.Print("caca");
    }

    public void StopCrouch()
    {
        _modelTargetScale = _modelInitScale;
        _colliderTargetScale = _colliderInitScale;
        OnStopCrouch?.Invoke(this, EventArgs.Empty);
        GD.Print("prout");
    }

    public void OnInputStart(object sender, EventArgs e)
    {/*
        if(GroundState.IsGrounded())
        {
            if(controller.RealVelocity.Length() > SlideMinSpeed)
            {

            }
        }*/

    }
    public override void _Process(double delta)
    {
        // To implement
    }
}