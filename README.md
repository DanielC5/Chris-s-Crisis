# Chris's Crisis

A 2D tube shooter inspired by the classic arcade game Gyruss, set in a surreal automotive nightmare. Built in Unity during CMU's NHSGA (National High School Game Academy) Lightning Round in Summer 2025 (1 week).

**Play it here:** [https://danielc87.itch.io/chriss-crysuss](https://danielc87.itch.io/chriss-crysuss)

## Gameplay

The player controls a car rotating around the perimeter of a tunnel, shooting inward at waves of hostile trees emerging from the vanishing point. The game uses a polar coordinate system — all movement and combat happen on a circular plane.

### Controls

| Input | Action |
|-------|--------|
| A / Left Arrow | Rotate clockwise |
| D / Right Arrow | Rotate counter-clockwise |
| Space / Click | Shoot toward center |

### Core Loop

1. Rotate around the tunnel perimeter to dodge enemies and projectiles
2. Shoot bullets inward to destroy tree enemies before they reach you
3. Collect pickups (speed boost, extra lives) dropped by defeated enemies
4. Clear 3 waves per stage, then advance to the next hour (level)
5. Face a boss tree at the end that spawns minions

### Enemy Types

- **Oak/Evergreen Trees** — Standard enemies that enter from the center, lock into ring slots, jitter in place, occasionally fire projectiles outward, and sometimes surge toward the player
- **Boss Tree** — Sits at the center, has 50 HP, and continuously spawns minions
- **Minions** — Smaller, faster enemies spawned by bosses that spiral outward

### Pickups

- **Speed** — 1.5x rotation speed
- **Health** — Extra life

## Project Structure

```
Assets/
├── Scripts/
│   ├── Game/
│   │   ├── PlayerController.cs      — Polar movement + shooting
│   │   ├── PlayerHealth.cs          — Player damage handling
│   │   ├── EnemyController.cs       — Enemy state machine (enter/align/stay/surge/retreat/exit)
│   │   ├── EnemyHealth.cs           — Enemy damage handling
│   │   ├── MinionController.cs      — Boss-spawned minion behavior
│   │   ├── InGameManager.cs         — Wave spawning, slot system, stage progression
│   │   ├── BulletBehaviour.cs       — Polar-space projectile movement + collision
│   │   ├── Boss/BossController.cs   — Boss HP, fade-in, minion spawning
│   │   └── Pickups/
│   │       ├── PickupController.cs  — Pickup movement (fade in, orbit, fly out)
│   │       └── PickupManager.cs     — Pickup wave spawning
│   ├── Utilities/
│   │   ├── PolarTransform.cs        — Core polar-to-cartesian coordinate system
│   │   ├── AnimationController.cs   — Car turn/death/respawn animations
│   │   ├── BackgroundController.cs  — Animated tunnel background
│   │   ├── RotateScript.cs          — Face-toward-center rotation
│   │   ├── RoadItemController.cs    — Scaling road elements for depth effect
│   │   ├── ShootingFXController.cs  — Muzzle flash sprites based on car direction
│   │   ├── StartInputHandler.cs     — Title screen input + scene loading
│   │   └── StaticUI.cs              — Persistent UI across scenes
│   ├── Interfaces/
│   │   └── IDamageable.cs           — Shared damage interface
│   ├── GameManager.cs               — Global state machine, lives, score, scene flow
│   ├── SoundManager.cs              — Centralized SFX playback
│   └── EnemySoundManager.cs         — Enemy-specific SFX
├── Scenes/
│   ├── 0_title.unity                — Title screen
│   ├── 1_game.unity                 — Level 1
│   ├── 2_game.unity                 — Level 2
│   ├── 3_game.unity                 — Boss level
│   ├── StartScene.unity             — Start scene
│   └── EndScene.unity               — Game over screen
├── Sprites/                          — Hand-drawn 2D art
├── Sounds/                           — Music and SFX
├── Prefabs/                          — Reusable game objects
├── Animations/                       — Animation controllers
└── Mixers/                           — Audio mixer settings
```

## Key Technical Systems

- **Polar Coordinate System** (`PolarTransform.cs`) — All gameplay objects (player, enemies, bullets, pickups) position themselves using (radius, angle) converted to cartesian coordinates each frame. This creates the tube/tunnel effect.
- **Wave/Slot System** (`InGameManager.cs`) — Enemies are spawned in waves and assigned to evenly-distributed ring slots via a FIFO queue. The ring breathes (radius oscillates) and rotates over time.
- **Enemy State Machine** (`EnemyController.cs`) — Enemies go through phases: Enter (spiral in from distance) -> Align (rotate to assigned slot) -> Stay (jitter + occasionally shoot/surge) -> Exit (spiral back out).
- **Game State Machine** (`GameManager.cs`) — Manages global flow: Start -> Loading -> Playing -> Died -> Respawning -> FinishedHour -> FinishedNight -> GameOver. Persists across scene loads.

## Running Locally

**Requirements:** Unity 6000.0.51f1

1. Clone the repo
2. Open in Unity Hub
3. Load `0_title` scene
4. Press Play

## Credits

- **Daniel Cai** — Programmer / Producer
- **Alan Yu** — Programmer
- **Kimi Ma** — Programmer
- **Alexa Zhang** — Artist
- **Chris Xiong** — Artist
- **Noah Kim** — Sound Designer

Made at CMU NHSGA, Summer 2025.
