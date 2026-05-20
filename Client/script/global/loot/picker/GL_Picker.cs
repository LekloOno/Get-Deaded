using Godot;

[GlobalClass]
public partial class GL_Picker : Area3D
{
    [Export] private PW_WeaponsHandler _weaponsHandler;

    [Signal]
    public delegate void DamageBuffPickedEventHandler(GL_DamageBuffData data);

    [Signal]
    public delegate void SlowMoPickedEventHandler(GL_SlowMoData data);

    [Signal]
    public delegate void SlowMoStartedEventHandler(float duration);

    [Signal]
    public delegate void SlowMoFadeInStartedEventHandler(float duration);

    [Signal]
    public delegate void EffectsCleansedEventHandler();

    public override void _Ready()
    {
        BodyEntered += ProcessCollision;
        Enable();
    }

    private void ProcessCollision(Node body)
    {
        if (body is GL_IPickable pickable)
            pickable.GetPicked(this);
    }

    public void Disable()
    {
        CollisionMask = 0;
    }

    public void Enable()
    {
        CollisionMask = CONF_Collision.Masks.Picker;
    }

    public void CleansEffects()
    {
        if (GL_SlowMoProcess.Active != null)
            GL_SlowMoProcess.Active.Abort(true);

        EmitSignal(SignalName.EffectsCleansed);
    }

    public bool PickAmmo(GL_AmmoData data) => _weaponsHandler.PickAmmo(data);

    public void PickDamageMultiplier(GL_DamageBuffData data)
    {
        _weaponsHandler.PickDamageMultiplier(data);
        EmitSignal(SignalName.DamageBuffPicked, data);
    }

    public void PickSlowMo(GL_SlowMoData data)
    {
        GL_SlowMoProcess? active = GL_SlowMoProcess.Active;
        if (active != null)
        {
            active.DurationStarted += TransmitSlowMoDuration;
            active.FadeInStarted += TransmitSlowMoFadeInStarted;
        }
            
        EmitSignal(SignalName.SlowMoPicked, data);
    }

    private void TransmitSlowMoDuration(float duration) =>
        EmitSignal(SignalName.SlowMoStarted, duration);

    private void TransmitSlowMoFadeInStarted(float duration) =>
        EmitSignal(SignalName.SlowMoFadeInStarted, duration);
}