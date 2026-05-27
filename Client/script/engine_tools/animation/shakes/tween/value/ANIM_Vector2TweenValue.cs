using Godot;

[GlobalClass]
public partial class ANIM_Vector2TweenValue : ANIM_TweenValue
{
    [Export] private Vector2 _value;
    public override Variant Value => _value;
}