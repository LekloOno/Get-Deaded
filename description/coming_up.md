This a little list of what is already planned for the game.

New features, systems, fixes, changes, balancing etc.

- [0.2](#02)
- [0.3](#03)
  - [News](#news)
    - [Content](#content)
      - [New map](#new-map)
      - [New mode(s)](#new-modes)
    - [UI](#ui)
      - [Shield/Barrier hit icon](#shieldbarrier-hit-icon)
      - [Amos location helper](#amos-location-helper)
  - [Changes](#changes)
    - [Enemies](#enemies)
      - [Target acquisition AI improvment](#target-acquisition-ai-improvment)
    - [Movement](#movement)
      - [Ledge Vault](#ledge-vault)
  - [Backend](#backend)
    - [Priority](#priority)
    - [Quality of life](#quality-of-life)
    - [Balance](#balance)
- [General](#general)
  - [News](#news-1)
    - [View models](#view-models)
  - [Changes](#changes-1)
    - [Controler Physics](#controler-physics)
      - [Walls interactions system rework](#walls-interactions-system-rework)
      - [Step climber](#step-climber)
      - [Slopes interactions rework](#slopes-interactions-rework)
      - [Lurch](#lurch)


# 0.2

Final v0.2 is out and ready as v0.2.3. Advancements are now focused on upcoming 0.3.

# 0.3

0.3 goal is to move from the intense lienar 80 seconds of pure fight, to an actual map with reasonable scale that starts to reflect the real game design intentions.

The player will have, not only to fight, but also to move around the map to get to interest points.

This version will explore the map scale, start to scratch a sense of game tempo/rythm, and eventually build a very basic game mode that makes scoring slightly more sublte, undirect, than just killing enemies.

## News

### Content

#### New map
Complete, medium scale map with intricated routes and various fight styles.

#### New mode(s)
Maybe explore  simple modes that are not purely based on kills to score, but something slightly more subtle to create a more interesting incentive. For example, time to clear, a control/hardpoint like mode, etc. Remain simple for now. 

### UI

#### Shield/Barrier hit icon
Add a little icon to add one more visual cues that different health acts differently when hit.

#### Amos location helper
When the player is low on amos, add a little indicator (configurable in user settings, to typically be turned off) of where the player can find amos, for example, the closest amo picker.

## Changes

### Enemies

#### Target acquisition AI improvment

**Vision**  
Make so enemies don't always know where you're at. Would typically allow to reach their back by using flank routes.

**Decision**  
Make so enemies try to reach for you when they know where you are, but are hidden, typically, if you were in plain sight and got back behind cover because you were low on health.

This is necessary, because this 0.3 will try to really put the player under health management pressure. It is currently too easy and unrealistic to use any small cover to fully regen in any dangerous situation, since bot don't try at all to open their sight.

**Quick note for future versions**  
This will start to open to a real state-machine like system. In short-term future version, this state machine could include - an idle state, where the bot has no information regarding the player, an aggression state, where the bot tries to engage a hidden player, a combat state, when he's engaged, and a flee state, when the bot is low on health and has opportunities to try and flee.

### Movement

#### Ledge Vault

To play test to confirm, but we might want ledge vault to be fully (or more) directed by the ledge direction. Currently, no matter the ledge, if the player goes very fast, and ledge climb, the normal of the ledge has very low impact on the direction of the vault, the prior velocity is prevalent. This feels odd in some situation - to play test to confirm.

## Backend

### Priority

Adapt the backend to include -
- Score time stamp - notably for versioning
- Game version
- Game mode

### Quality of life

**Launcher**  
Develop a game launcher to share the project easily. Handle versions, update, and embed patch notes.

**Additionnal general meta-data**  
Associate more meta data for map-mode-run entries.

For example, register enemy killed types, for sector mode - add which sector where played, etc.

**Time tracked score**  
Track score evolution over time, and eventually associate this to some meta-data, for example, how much time spent on X sector.

### Balance

Easy mode in 0.2.3 was already hard for some people, and hard mode for 80 seconds straight can be exhausting and not that fun, even for veterans.

For v0.3, we will try to achieve a better balance.

# General

## News

### View models

Weapons, hands, with their beloved animation. It is of course a very important point, but also quite of an expensive one, so it is delayed as much as possible for now. But it is highly regarded, it has a significant impact on the general game feel, and will eventually come.

## Changes

### Controler Physics

#### Walls interactions system rework

The wall interactions - wall bounce, wall climb, ledge climb, etc. - will be reworked into a more stable system and centralized system. The goal is to make the whole system more intuitive, while making it even deeper, and also to bind this to more cues like camera animations and more.

#### Step climber

The current character controler does not include a step climber - a system that makes so the player can climb little steps like if it was smooth slopes, stairs, etc.

This can already lead currently to a few edge case scenarios that makes the controler movement not smooth, like some slopes that have super tiny geometry artifacts, enough for the player to stuck on them.

#### Slopes interactions rework

Slopes interactions aren't well handled yet. There's interesting ones, but it is not yet working exactly like intended, the player typically don't go any faster on downard slopes while sliding, unless he's already fast enough to not be considered grounded.

It was a very early, unfinished part of the controler physics, that need to be well integrated in a solid base system. It will typically come along with the step climber.

#### Lurch

Lurches were an attempt to provide a way to redirect momentum in a very niche but skillfull way, in a similar fashion to apex tap straffing. The current lurch ability isn't super interesting, nor deep, as tap straffing is.

Lurch might be kept just for the vertical momentum redirection, this specific part is actually interesting.

For the horizontal redirection, another mechanic will have to be worked on, to better match the depth of something like tap straffing and derivatives.