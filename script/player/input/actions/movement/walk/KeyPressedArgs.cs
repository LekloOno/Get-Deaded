using System;
using Godot;

namespace Pew;

public class KeyPressedArgs : EventArgs
{
    private Vector3 _wishDir;
    private Vector2 _walkAxis;

    public KeyPressedArgs(Vector3 wishDir, Vector2 walkAxis)
    {
        _wishDir = wishDir;
        _walkAxis = walkAxis;
    }

    public Vector3 WishDir {get => _wishDir;}
    public Vector2 WalkAxis {get => _walkAxis;}
}