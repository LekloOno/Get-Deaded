using Godot;

[GlobalClass]
public partial class PIM_Arena : Node
{
    [Export] private PM_Controller _controller;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_Sprint _sprintInput;
    [Export] private PI_CrouchDispatcher _crouchDispatcherInput;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private PI_Revive _reviveInput;
    
    public override void _Ready()
    {
        SetAlive();

        _controller.OnDie += (o, e) => SetDead();
        _reviveInput.Revive += (o, e) => SetAlive();
    }
    public void SetAlive()
    {
        _controller.Revive();

        _controller.SetProcessUnhandledInput(true);
        _walkInput.EnableAction();
        _sprintInput.EnableAction();
        _crouchDispatcherInput.EnableAction();
        _weaponsInput.EnableAction();

        _reviveInput.DisableAction();
    }

    public void SetDead()
    {
        _controller.SetProcessUnhandledInput(false);
        _walkInput.DisableAction();
        _sprintInput.DisableAction();
        _crouchDispatcherInput.DisableAction();
        _weaponsInput.DisableAction();

        _reviveInput.EnableAction();
    }
}