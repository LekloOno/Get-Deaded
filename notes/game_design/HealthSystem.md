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

<table>
  <tr>
    <th>Layer</th>
    <th>Body Color</th>
    <th>Tail Color</th>
    <th>Heal Color</th>
  </tr>
  <tr>
    <td>Health</td>
    <td style="background-color: white;">_</td>
    <td style="background-color: red;">_</td>
    <td style="background-color: rgb(170, 255, 170);">_</td>
  </tr>
  <tr>
    <td>Armor</td>
    <td style="background-color: rgba(255, 191, 71, 1);">_</td>
    <td style="background-color: lightgrey;">_</td>
    <td style="background-color: rgb(170, 255, 170);">_</td>
  </tr>
  <tr>
    <td>Barrier</td>
    <td style="background-color: rgb(90, 19, 255)">_</td>
    <td style="background-color: lightgrey;">_</td>
    <td style="background-color: rgb(170, 255, 170);">_</td>
  </tr>
  <tr>
    <td>Shield</td>
    <td style="background-color: rgb(0, 102, 255);">_</td>
    <td style="background-color: lightgrey;">_</td>
    <td style="background-color: rgb(170, 255, 170);">_</td>
  </tr>
  <tr>
    <td>OverShield</td>
    <td style="background-color: rgb(0, 240, 0);">_</td>
    <td style="background-color: lightgrey;">_</td>
    <td>None</td>
  </tr>
  <tr>
    <td>Shell</td>
    <td style="background-color: rgb(80, 80, 80);;">_</td>
    <td style="background-color: lightgrey;">_</td>
    <td>None</td>
  </tr>
</table>

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
possible in game modification, influence ..

## Algorithm
how it works, variable parameters ..

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