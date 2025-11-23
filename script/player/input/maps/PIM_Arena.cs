using Godot;

[GlobalClass]
public partial class PIM_Arena : Node
{
    [Export] private PM_Controller _controller;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_Sprint _sprintInput;
    [Export] private PI_Jump _jumpInput;
    [Export] private PI_CrouchDispatcher _crouchDispatcherInput;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private PI_Revive _reviveInput;
    [Export] private PI_Stats _statsInput;

    private PI_Map _alive;
    private PI_Map _dead;
    
    public override void _Ready()
    {
        _alive = [_walkInput, _sprintInput, _jumpInput, _crouchDispatcherInput, _weaponsInput];
        _dead = [_reviveInput];
        _statsInput.EnableAction();
        
        Alive();

        _controller.OnDie += (o, e) => Dead();
        _reviveInput.Revive += (o, e) => Alive();
    }
    public void Alive()
    {
        _controller.Revive();
        _dead.Disable();
        _alive.Enable();
    }

    public void Dead()
    {
        _alive.Disable();
        _dead.Enable();
    }
}