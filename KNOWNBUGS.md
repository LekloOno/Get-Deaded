RealVelocity is sometimes very off. When moving straight against a wall, then jumping, the very first tick of the jump has a force of ~8, where it should be ~3. The Capsule must be moved up some way by the physics engine, resulting in our physics to think the player was moving much faster than he was.
It causes issue one RealVelocity is used as a based velocity, which is done with the VelocityCache.

Drag Is not **Fully** Tick Rate independant ( - successive multiplications - ) Might be fixable with a similar method than the tick rate independant lerping method.
```cs
_independantLerpSpeed = 1f - Mathf.Exp(-lerpSpeed*(float)GetPhysicsProcessDeltaTime());
Mathf.Lerp(current, target, _independantLerpSpeed);
```

When timing a jump at the right moment while uncrouching, you can get significantly increased jump height.
It's not that easy to do, and kind of intuitive, so it might be kept as a feature. Need to check if it is not too unpredictable, tick rate independant, etc.