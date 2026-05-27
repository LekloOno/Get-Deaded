using Godot;

[GlobalClass]
public partial class ANIM_InOutTweenSetting : Resource
{
    [Export] public ANIM_TweenSetting? FadeIn {get; private set;}
    [Export] public ANIM_TweenSetting? FadeOut {get; private set;}
}