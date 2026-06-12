using Godot;

[GlobalClass]
public partial class DATA_DoubleJump : Resource
{
    [Export] public float ChargeCost        { get; private set; } = 66.7f;
    [Export] public float Strength          { get; private set; } = 38f;
    [Export] public float HorizontalPenalty { get; private set; } = 0.18f;
    [Export] public float VerticalPenalty   { get; private set; } = 0.5f;
    [Export] public float Duration          { get; private set; } = 0.28f;
}