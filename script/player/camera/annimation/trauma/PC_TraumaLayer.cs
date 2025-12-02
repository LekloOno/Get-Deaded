using Godot;

[GlobalClass]
public partial class PC_TraumaLayer : ANIM_TraumaLayer<Vector3>
{
    protected override Vector3 Zero() => Vector3.Zero;
    protected override Vector3 ShakeVector(float intensity, float time) =>
        new(
            intensity * GetNoiseFromSeed(0, time),
            intensity * GetNoiseFromSeed(1, time),
            intensity * GetNoiseFromSeed(2, time)
        );
}