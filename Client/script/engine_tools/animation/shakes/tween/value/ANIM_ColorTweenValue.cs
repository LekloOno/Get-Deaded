using Godot;

[GlobalClass]
public partial class ANIM_ColorTweenValue : ANIM_TweenValue
{
    [Export] private Color _value;
    public override Variant Value => _value;
}