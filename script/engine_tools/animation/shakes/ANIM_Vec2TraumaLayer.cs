using Godot;

namespace Pew;

[GlobalClass]
public partial class ANIM_Vec2TraumaLayer : ANIM_TraumaLayer<Vector2>
{
    protected override Vector2 Zero() => Vector2.Zero;
    protected override Vector2 ShakeVector(float intensity, float time) =>
        new(
            intensity * GetNoiseFromSeed(0, time),
            intensity * GetNoiseFromSeed(1, time)
        );
}