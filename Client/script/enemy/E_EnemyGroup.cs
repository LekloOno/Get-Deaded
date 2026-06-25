using Godot;
using Godot.Collections;

[GlobalClass]
public partial class E_EnemyGroup : Resource
{
    [Export] private Array<E_EnemyBuilder> _enemies = null!;
}