# Health System
A quick discussion on how health will be implemented in the game.

# Health pools

The different health pools are as follows. The lower in the list is the highest priority in damage intake.
- **Basic Health pool** - Simple health.
- **Armor** - Weak against burst damage, strong against sustained damage.
- **Barrier** - Weak against sustained damage, strong against burst damage.
- **Shield** - Simple health that can regenerate over time.
- **Overshield** - A temporary pool of simple health that cannot be restored, and comes from pick ups or abilities.

An **Entity** is dead when its lower layer of health is 0. In most cases, it would be the **Basic Health pool** but it isn't necessary the case.  
If an entity has no starting basic health, yet has Armor, then only this layer would determine if the entity is dead or alive.

**Ennemies** can have any combination of health pool, and can also have **plating**.  
It decreases the overall damage intakes, but can be reduced or broken with special abilities or weapons.  

**Player** has Basic **Health** and a little pool of **Shield**. Through pickups or abilities, he can also obtain ***Overshield**.

# Basic Health pool

It is the last possible layer of health.

## Healing

### Player

To heal its health pool, the player must pick up some health packs, and use them.  
The healing process would take a few seconds, and doing so, the player's move speed would be slightly reduced plus he might not be able to sprint.

Some abilities would also heal the player.

### Ennemies

There might be healers ennemies, which heals their allies, which could be some priority targets.

## Game Feel

# Armor

## Algorithm
how it works, variable parameters ..

## Game Design
possible in game modification, influence ..

## Game Feel
discuss sounds, ui ..


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