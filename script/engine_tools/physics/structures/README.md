/[scripts](../../../)/[engine_tools](../../)/[physics](../)/structures  

---


This folder contains general purpose structures that simplify some physics manipulations.

- [ForcesCache](#forcescache)
  - [Impulse Forces](#impulse-forces)
  - [Persistent Forces](#persistent-forces)
  - [Methods](#methods)


# [ForcesCache](PHX_ForcesCache.cs)

A tool to cache and consume forces, typically for some exterior actors to be able to apply forces to another body while keeping a tight grip on how the force are internally applied.  
Instead of risking concurrent modifications, and not to be sure when they are applied, the actor holding this forcesCache is reponsible for consuming it when it wants to.  

See usage example in -
- [PM_Controller](../../player/PM_Controller.cs) - Controller is the holder of the cache.
- [PM_Slide](../../player/movement/actions/crouch/PM_Slide.cs) - Slide adds some impulse forces.
- [PM_Dash](../../player/movement/actions/crouch/PM_Dash.cs) - Dash adds some persistent forces.

## Impulse Forces

Consuming these forces clear them. These forces will thus be applied only once. They are used for ponctual forces, like an explosion, a hit, a jump, etc.

## Persistent Forces

Consuming these forces do not clear them. They allow for long lasting forces, but should then be removed explicitly by the actor responsible of its addition.

## Methods

- **IsEmpty** - returns true if no forces are currently cached, false otherwise.
- **ConsumeImpulse** - Consume and clears all cached impulse forces, returning their sum.
- **ConsumePersistent** - Consume all cached persistent forces, without clearing them, returning their sum.
- **Consume** - Consume all forces. Same as calling ``ConsumeImpulse`` then ``ConsumePersistent``.
- **AddImpulse** - Adds an impulse force to the cache.
- **AddPersistent** - Adds a persistent force to the cache.
- **RemovePersistent** - Removes a persistent force to the cache.

