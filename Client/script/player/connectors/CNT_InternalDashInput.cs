using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class CNT_InternalDashInput : Node
{
    [Export] private PI_Dash _dashInput = null!;
    private readonly PI_Dash _internal = new();

    private readonly List<Func<bool>> _handlers = [];
    public event Func<bool> Dashed
    {
        add
        {
            if (_handlers.Count == 0)
                Enable();

            _handlers.Add(value);
        }

        remove
        {
            _handlers.Remove(value);

            if (_handlers.Count == 0)
                Disable();
        }
    }

    public override void _Ready()
    {
        _dashInput.GetParent().RemoveChild(_dashInput);
        AddChild(_dashInput);
        _internal.OnStartInput += OnInternalDashStart;
    }

    private void OnInternalDashStart(object? sender, EventArgs e)
    {
        if (!Handled())
            _dashInput.KeyDown();   
    }

    private bool Handled()
    {
        foreach (Func<bool> handler in _handlers)
            if (handler())
                return true;

        return false;
    }

    private void Enable()
    {
        RemoveChild(_dashInput);
        AddChild(_internal);
    }

    private void Disable()
    {
        RemoveChild(_internal);
        AddChild(_dashInput);
    }
}