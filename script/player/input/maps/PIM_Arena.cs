using System;
using Godot;

namespace Pew;

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
    [Export] private bool _useMouseCursor = false;
    [Export] private bool _pauseGame = true;

    public Action OnMenuOpen;
    public Action OnMenuClose;

    private PI_Map _alive;
    private PI_Map _dead;
    private PI_Map _escapeMenu;

    private PI_Map _activeGameMap;
    private bool _menu = false;

    private Input.MouseModeEnum _prevMouseMode;
    
    public override void _Ready()
    {
        _alive = [_walkInput, _sprintInput, _jumpInput, _crouchDispatcherInput, _weaponsInput];
        _dead = [_reviveInput];
        _escapeMenu = [];

        _statsInput.EnableAction();
        
        Alive(null, 0f);
        _activeGameMap = _alive;
        _prevMouseMode = Input.MouseMode;

        _controller.OnDie += Dead;
        _controller.OnDie += TrackDead;
        _reviveInput.Revive += Alive;
        _reviveInput.Revive += TrackAlive;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_cancel"))
            return;

        _menu = !_menu;
        
        if (_menu)
            MenuShow();
        else
            MenuHide();
    }

    public void TrackAlive(object _, float __) =>
        _activeGameMap = _alive;
    public void TrackDead(object _, EventArgs __) =>
        _activeGameMap = _dead;

    public void Alive(object _, float __)
    {
        _controller.Revive();
        _dead.Disable();
        _alive.Enable();
    }

    public void Dead(object _, EventArgs __)
    {
        _alive.Disable();
        _dead.Enable();
    }

    private void MenuShow()
    {
        _activeGameMap.Disable();
        _controller.OnDie -= Dead;
        _reviveInput.Revive -= Alive;

        if (_useMouseCursor)
        {
            _prevMouseMode = Input.MouseMode;
            Input.WarpMouse(GetViewport().GetVisibleRect().Size / 2f);
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (_pauseGame)
            GetTree().Paused = true;
        
        _statsInput.DisableAction();

        _escapeMenu.Enable();
        OnMenuOpen?.Invoke();
    }

    private void MenuHide()
    {
        _escapeMenu.Disable();

        if (_useMouseCursor)
            Input.MouseMode = _prevMouseMode;

        if (_pauseGame)
            GetTree().Paused = false;

        _activeGameMap.Enable();
        _controller.OnDie += Dead;
        _reviveInput.Revive += Alive;

        _statsInput.EnableAction();
        OnMenuClose?.Invoke();
    }
}