using Godot;

[GlobalClass]
public partial class DATA_FrenzyEntry : Resource
{
    [Export] public E_EnemyBuilder Builder  { get; private set; } = null!;
    [Export] public uint           Count    { get; private set; } = 1;
}