RealVelocity is sometimes very off. When moving straight against a wall, then jumping, the very first tick of the jump has a force of ~8, where it should be ~3. The Capsule must be moved up some way by the physics engine, resulting in our physics to think the player was moving much faster than he was.
It causes issue one RealVelocity is used as a based velocity, which is done with the VelocityCache.

Drag Is not **Fully** Tick Rate independant ( - successive multiplications - ) Might be fixable with a similar method than the tick rate independant lerping method.
```cs
_independantLerpSpeed = 1f - Mathf.Exp(-lerpSpeed*(float)GetPhysicsProcessDeltaTime());
Mathf.Lerp(current, target, _independantLerpSpeed);
```

When timing a jump at the right moment while uncrouching, you can get significantly increased jump height.
It's not that easy to do, and kind of intuitive, so it might be kept as a feature. Need to check if it is not too unpredictable, tick rate independant, etc.

LedgeClimb stops right when the foot ray does not collide anymore. However, there might be small holes on the obstacles to climb right at foot level, which blocks the ledgeclimb.


## Edge fall Bug

Sliding of an edge makes you fall down super fast. Maybe something is wrong with the way gravity is applied ?
    - Works without sliding, but for a different reason.

### Slide
Probably a collision issue which results in RealVelocity being unstable, same as the front wall jump bug.

### Normal
~~When velocity Y is upward, move and slide does not modify Velocity - maybe it is linked~~
~~At the end of the ledge climb, player has a slight upward bounce ?~~ 
        
~~Maybe the ledge climb can end before being properly on top of the obstacle, the player falls, hits his feet on the very last bit of the edge which is considered a wall, manages to pass over as it is very small, and now it has downward cached velocity. Because it has downward velocity, moveandslide does not update the velocity and gravity keeps getting applied. If the player goes out of the platform, the gravity is released.~~

**Observations**
```sh
AT 6100 : -1.02..   # Normal gravity applied after the ledge climb - why is it applied though ?
AT 6105 : -1.08..
AT 6111 : -1.14..
AT 6342 : -5.15..   # SUDDEN JUMP ! corresponds to the down bug - not correlated to the velocity cache ..
AT 6348 : -5.22..
```
It happens consistently if right against the wall to climb, then sprinting. You can sprint after climbing, or right before ending the climb.
It only works with precisely the same surface data settings as the sprint, the speed, the accel AND the drag. (speed and accel equals wouldn't result in this bug, drag and accel either, etc..)
The y velocity of the player is affected by gravity, then is set to 0 for ~200 ms, then it is suddenly set to -5.
This jump DOES NOT corresponds to the velocity it would have if gravity kept being applied under the radar during this 200 ms window. It would be ~ -3.4.

**Hypothesis**
Maybe the velocity is projected on the edge of the obstacle, leading in high vertical speed. The next tick, player is not considered grounded because of it, the base velocity is now RealVelocity, so move and slide does not update velocity anymore. - But why would that only happen when climbing against the wall and with specifics sprinting data ?

**Highest probability** : Same cause that for the front wall jump bug. RealVelocity would be unstable.





the slide snapping is about the collide and slide algorithm. at max angle 0, the bug doesn't happen.
It's not floor snap, it might be but its height doesn't affect how strong the bug is.


The sprint snap bug occurs with just drag and surface control enabled, even with no air control. But it doesn't give any non flat normal.
However, when snapping down it doesn't count as a collision (printing collisions when moving down a slope, although snapped, does not give any collsiion)