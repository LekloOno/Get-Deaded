/[scripts](../../../)/[engine_tools](../../)/[physics](../)/static  

---

This folder contains static classes that offer general purpose physics static functions tools.

- [Checks](#checks)
  - [CanUncrouch](#canuncrouch)
  - [CanMoveForward](#canmoveforward)
  - [CanLedgeClimb](#canledgeclimb)
- [MovementPhysics](#movementphysics)
  - [Acceleration](#acceleration)


# [Checks](PHX_Checks.cs)

A static class with various spatial checking methods, typically to avoid clipping, verifying that some movement is possible, etc. 

## CanUncrouch

Checks if a given body can scale up to its original scale without clipping into collisions. `upSafeMargin` allow to add an additionnal margin to restrict even further the check.  
The higher the `upSafeMargin`, the more likely it is that the function will return false (can't scale up the target).

## CanMoveForward

Checks that a given physics body can move forward without clipping into collisions. 
- `current` the body to check. 
- `shape` the shape of `current` - is given to briefly modify its size if a `feetMargin` is given.  
- `pivot` is the view pivot (camera pivot if it's a player), that is, the direction in which the entity is looking.  
- `distance` is the distance to test the movement on.  
- `result` contains the informations about the collisions if there is some, empty otherwise.  
- `feetMargin` Allows to consider an extra permissive margin to ignore the feet of the entity's collisions.


## CanLedgeClimb

Checks if an entity can ledge climb - that is, it can't move forward (meaning there's an obstacle in front of it) BUT its ledge cast is free (~ there's nothing in front of him at head level).
- `current` the body to check.
- `shape` is given to briefly modify its size if a `minHeight` != 0 is given.  
- `pivot` is the view pivot (camera pivot if it's a player), that is, the direction in which the entity is looking.  
- `distance` is the distance to test the movement on. 
- `headCast` is a ShapeCast to detect if the head-level is free for a ledgeclimb.
- `minHeight` the minimum height of an obstacle that can be ledge climbed.
- `result` contains the informations about the collisions if there is some, empty otherwise.  


# [MovementPhysics](PHX_MovementPhysics.cs)

Some static functions to handle specific movement physics.

## Acceleration

The Holy quake acceleration.