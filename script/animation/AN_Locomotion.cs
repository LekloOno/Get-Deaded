using System;
using Godot;

[GlobalClass]
public partial class AN_Locomotion : Node
{
    [ExportCategory("References")]
    [Export] private GB_CharacterBody _body;
    [Export] private GM_Mover _mover;
    [Export] private AnimationTree _animationTree;
    [Export] private string _locomotionBlendPath;
    [Export] private string _timeScaleBlendPath;

    /// <summary>
    /// The speed at which the animation will fully play the walk animation. <br/>
    /// <br/>
    /// From 0 to this speed, the animation blends between idle and walk.
    /// </summary>
    [ExportCategory("Animation Config")]
    [Export] private float _fullWalkSpeed = 2f;
    /// <summary>
    /// The speed at which the animation will start blending from walking to sprinting. <br/>
    ///  <br/>
    /// From _fullWalkSpeed to this speed, the animation remains fully in walk animation. <br/>
    /// From this speed to _fullSprintSpeed, the animation blends between walk and sprint. 
    /// </summary>
    [Export] private float _minSprintSpeed = 3f;
    /// <summary>
    /// The speed at which the animation will fully play the sprint animation. <br/>
    /// <br/>
    /// From _minSprintSpeed to this speed, the animation blends between walk and sprint.
    /// </summary>
    [Export] private float _fullSprintSpeed = 5f;
    
    /// <summary>
    /// Add lag to the actual current state of animation. <br/>
    /// <br/>
    /// 1 means no lag at all - it can result in snappy transition. <br/>
    /// 0 means the animation will never be updated.
    /// </summary>
    [ExportCategory("Animation Config")]
    [Export(PropertyHint.Range, "0,1,0.01")]
    private float _animationTransitionSpeed = 0.08f;
    /// <summary>
    /// Add lag to the speed currently considered to scale the animation speed. <br/>
    /// <br/>
    /// 1 means no lag at all - it can result in snappy transition. <br/>
    /// 0 means the animation speed will never be updated.
    /// </summary>
    [Export(PropertyHint.Range, "0,1,0.01")]
    private float _scaleTransitionSpeed = 0.08f;
    /// <summary>
    /// This is the x/y value at which the blendspace 2D is in full walk mode. <br/>
    /// Check the linked animation tree locomotion's BlendSpace2D to match. <br/>
    /// </summary>
    [Export] private float _blendSpaceWalkStart = 0.5f;
    /// <summary>
    /// This is the time scale that should be applied to the animation at full sprint speed. <br/>
    /// </summary>
    [Export] private float _sprintAnimSpeed = 2f;
    /// <summary>
    /// This is the time scale that should be applied to the animation at full walk speed. <br/>
    /// </summary>
    [Export] private float _walkAnimSpeed = 1f;

    private Vector2 _smoothedAnimPos = Vector2.Zero;
    private float _smoothedSpeed = 0f;

    public override void _Ready()
    {
        if (_animationTree == null)
        {
            SetPhysicsProcess(false);
            return;
        }

        try
        {
            _animationTree.Get(_locomotionBlendPath);
            _animationTree.Get(_timeScaleBlendPath);
        }
        catch(Exception)
        {
            SetPhysicsProcess(false);
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        float flatSpeed = MATH_Vector3Ext.Flat(_body.Velocity).Length();

        // Animation
        float animationSpeedRatio = AnimationStateSpeedRatio(flatSpeed);

        Vector2 newPos = animationSpeedRatio * _mover.WalkAxis;
        
        _smoothedAnimPos = _smoothedAnimPos.Lerp(newPos, _animationTransitionSpeed);
        _animationTree.Set(_locomotionBlendPath, _smoothedAnimPos);

        // Speed Scale
        _smoothedSpeed = Mathf.Lerp(_smoothedSpeed, flatSpeed, _scaleTransitionSpeed);
        
        float animSpeedRatio = (_smoothedSpeed - _fullWalkSpeed) / (_fullSprintSpeed - _fullWalkSpeed);
        float animSpeed = Mathf.Lerp(_walkAnimSpeed, _sprintAnimSpeed, animSpeedRatio);
        
        _animationTree.Set(_timeScaleBlendPath, animSpeed);
    }

    
    /// <summary>
    /// We use _minSprint to allow for an explicit margin of speed in which we are fully walking <br/>
    /// instead of always gradually getting closer to a sprint. A plateau of walking. <br/>
    ///                 <br/>
    /// ^               <br/>
    /// | ---- Sprint   <br/>
    /// | - transition  <br/>
    /// | ---- walk     <br/>
    /// | walk          <br/>
    /// | ---- walk     <br/>
    /// | - transition  <br/>
    /// | ---- idle     <br/>
    /// </summary>
    /// <param name="flatSpeed"></param>
    /// <returns></returns>
    private float AnimationStateSpeedRatio(float flatSpeed)
    {
        // Above full sprint speed, fully play sprint
        if (flatSpeed > _fullSprintSpeed)
            return 1f;
        

        // Above min sprint speed, lerp between sprint and walk
        if (flatSpeed > _minSprintSpeed)
        {
            float walkToSprintRatio = (flatSpeed - _minSprintSpeed) / (_fullSprintSpeed - _minSprintSpeed);
            return walkToSprintRatio + _blendSpaceWalkStart;
        }

        // Fast enough to fully walk
        if (flatSpeed >= _fullWalkSpeed)
            return _blendSpaceWalkStart;

        // Not fast enough, blend between idle and walk
        return _blendSpaceWalkStart * flatSpeed / _fullWalkSpeed;
    }
}