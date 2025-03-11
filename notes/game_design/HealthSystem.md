# Health System
A quick discussion on how health will be implemented in the game.

# Health pools

The different health pools are as follows. The lower in the list is the highest priority in damage intake.
- **Basic Health pool** - Simple health.
- **Armor** - Weak against burst damage, strong against sustained damage.
- **Barrier** - Weak against sustained damage, strong against burst damage.
- **Shield** - Simple health that can regenerate over time.
- **Overshield** - A temporary pool of simple health that cannot be restored, and comes from pick ups or abilities.
- **Shell** - A simple health pool that can't be peirced through, and makes its owner immune to any negative effect, such as slow, weakening, etc.

An **Entity** is dead when its lower layer of health is 0. In most cases, it would be the **Basic Health pool** but it isn't necessary the case.  
If an entity has no starting basic health, yet has Armor, then only this layer would determine if the entity is dead or alive.

**Ennemies** can have any combination of health pool, and can also have **plating**.  
It decreases the overall damage intakes, but can be reduced or broken with special abilities or weapons.  

**Player** has Basic **Health** and a little pool of **Shield**. Through pickups or abilities, he can also obtain ***Overshield**.

# General game feel

Each layer of health could have
- a slight variant of the hitmarker sound
- a sound on layer breaking
- a specific health bar color

| Layer  | Body Color               | Tail Color           | Heal color |
| ------ | ------------------------ | -------------------- | ---------- |
| Health | ![#f0f0f0](https://placehold.co/15x15/f0f0f0/f0f0f0.png) | ![#f03c15](https://placehold.co/15x15/f03c15/f03c15.png) | ![#88ff78](https://placehold.co/15x15/88ff78/88ff78.png) |
| Armor | ![#ebb434](https://placehold.co/15x15/ebb434/ebb434.png) | ![#b0b0b0](https://placehold.co/15x15/b0b0b0/b0b0b0.png) | ![#88ff78](https://placehold.co/15x15/88ff78/88ff78.png) |
| Barrier | ![#6200ff](https://placehold.co/15x15/6200ff/6200ff.png) | ![#b0b0b0](https://placehold.co/15x15/b0b0b0/b0b0b0.png) | ![#88ff78](https://placehold.co/15x15/88ff78/88ff78.png) |
| Barrier | ![#006aff](https://placehold.co/15x15/006aff/006aff.png) | ![#b0b0b0](https://placehold.co/15x15/b0b0b0/b0b0b0.png) | ![#88ff78](https://placehold.co/15x15/88ff78/88ff78.png) |
| Over Shield | ![#00c421](https://placehold.co/15x15/00c421/00c421.png) | ![#b0b0b0](https://placehold.co/15x15/b0b0b0/b0b0b0.png) | None |
| Shell | ![#4a4a4a](https://placehold.co/15x15/4a4a4a/4a4a4a.png) | None | None |

We could have two health bar to display, the lower layer, and then a bar for all the higher special layers, each layer stacking on the lower one. We could also add a small icon on top of this bar to indicate which layers this entity has left.  

![alt text](image.png)

The carapace would take the entire bar space, until it is broken.

![alt text](image-1.png)

## An other approach

Many health bars like this might not be so intuitive. Another approach than having colored health bar would be to display it with a simple icon + on hit.

Change the color of the hitmarker or the damage indicator when the damage pool is resisting to the type of damage you are inflicting.

For example, if you shoot with a burst weapon on a barrier pool, the damage indicator would turn purple.

# Basic Health pool

It is the last possible layer of health.

## Healing

### Player

To heal its health pool, the player must pick up some health packs, and use them.  
The healing process would take a few seconds, and doing so, the player's move speed would be slightly reduced plus he might not be able to sprint.

Some abilities would also heal the player.

### Ennemies

There might be healers ennemies, which heals their allies, which could be some priority targets.

# Armor

## Game Design
Armor incentivize the player to use burst weapon, toward the use of his dynamic clicking technique.  
It could be interesting to have it vary, simply keep the core principle of making it stronger against sustained damage, but allow the specifics about it to vary from one ennemy type to another.

We could set how strong and what threshold the armor has. Meaning how much it can reduce the damages, and what is the burst threshold.

## Algorithm
There are mutliple solution to this idea.


### Linear reduction

[Desmos Visualization](https://www.desmos.com/calculator/p9je6b1tga?lang=fr)  
The simplest solution is to define reduction as a `resistance` % which is capped by a `max_reduction`.  
- `resistance` is the intended % of resistance.
- `max_reduction` is the maximum damage reduction that can be applied.

It means once the max reduction is passed, the higher the damage, the lower the **effective** resistance %.

The computation would be :
- `reduction = min(damage * (resistance), max_reduction)`
- `final_damage = damage - reduction`.


### Gradual reduction

[Desmos Visualization](https://www.desmos.com/calculator/orecuaomyh?lang=fr)  
A more complex idea, is to set a `maximum_resistance` %, a `minimum_threshold` and a `maximum_threshold`.
- `maximum_resistance` is the % of reduction at the best effectiveness of the armor.
- `minimum_threshold` is the minimum amount of damage for the **effective** resistance to start decaying below `maximum_resistance`.
- `maximum_threshold` is the mimimum amount of damage for the **effective** resistance to be 0.

Now, the reduction % gradually decreases from `minimum_threshold` to `maximum_threshold`, which might be more flexible and easier to balance, but it might also be a little less intuitive.

The computation would be :  

Compile time -
- `a = maximum_resistance/(minimum_threshold-maximum_resistance)`
- `b = `  

Run time -
- `resistance = clamp(damage * a + b, 0, maximum_resistance)`
- `reduction = damage * reduction`
- `final_damage = damage - reduction`


# Barrier

## Algorithm
how it works, variable parameters ..

## Game Design
possible in game modification, influence ..

## Game Feel
discuss sounds, ui ..

# Shield

## Algorithm
how it works, variable parameters ..

## Game Design
possible in game modification, influence ..

## Game Feel
discuss sounds, ui ..

# Overshield

## Algorithm
how it works, variable parameters ..

## Game Design
possible in game modification, influence ..

## Game Feel
discuss sounds, ui ..


# Plating

## Algorithm
how it works, variable parameters ..

## Game Design
possible in game modification, influence ..

## Game Feel
discuss sounds, ui ..