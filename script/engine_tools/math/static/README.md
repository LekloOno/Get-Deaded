/[scripts](../../..)/[engine_tools](../..)/[math](..)/static

---

This folder contains static maths formulas for various manipulations, not present in the standard C# and Godot library.

# [Sound](MATH_Sound.cs)

Contains maths static function related to sound.

## LerpDB

A function to interpolate *pseudo-linearly* between two decibel value. The function takes into account the logarithmic scale of decibels, to interpolate it "linearly" from a human ear perspective.

```cs
public static float LerpDB(float dB1, float dB2, float t);
```
Parameters :
- **dB1** - such as `LerpDB(dB1, dB2, 0) = dB1`
- **dB2** - such as `LerpDB(dB1, dB2, 1) = dB2`
- **t** - the interpolation coeficient, between 0 and 1.

# [Vector3Ext](MATH_Vector3Ext.cs)

Some extension functions to manipulate `Vector3`.

## SmoothDamp

Gradually transforms a vector towards the target vector over time, in a spring-like damper function.

```cs
public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float deltaTime);
```

Parameters :
- **current** - the current ``Vector3``.
- **target** - the target `Vector3`.
- **currentVelocity** - represents the velocity of the spring-like motion. It will be modified by the function, and should thus be passed as a reference.
- **smoothTime** - the approximate time required to reach the target.
- **deltaTime** - the time between calls of this function.

## Flat

A simple quickhand to flatten a vector horizontally, as it can be a little verbose.  
It basically returns a vector with only the X and Z axis of the input vector. The Y axis will always be 0.

