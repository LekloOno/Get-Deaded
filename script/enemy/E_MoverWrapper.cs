using Godot;

[GlobalClass]
public partial class E_MoverWrapper : GM_Mover
{
    [Export] private PROTO_Mover _mover;
    [Export] private E_Enemy _enemy;

    public override Vector3 Velocity => _enemy.Velocity;

    public override Vector3 WishDir => _mover.WishDir;

    public override Vector2 WalkAxis => _mover.WalkAxis;
}