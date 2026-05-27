using Godot;

[GlobalClass]
public partial class ANIM_FloatTweenValue : ANIM_TweenValue
{
    [Export] private float _value;
    public override Variant Value => _value;
}