# Apoapsis 2

A 2D mobile orbital puzzle game built in Unity 6. Pilot a rocket, achieve stable orbits around every moon in each level, and collect coins along the way.

## Gameplay

Launch from a planet's surface, time your burns, and use gravity to sling between moons. One-touch controls: hold to thrust. The challenge comes from reading trajectories, timing orbital transfers, and navigating hazards.

- 45 levels across 3 chapters
- Procedural planet/moon visuals with 10 shader-driven body styles
- Asteroids, satellites, and other orbital hazards
- Score system with near-miss bonuses, coin collection, and per-level high scores
- iOS haptic feedback

## Tech Stack

- **Engine:** Unity 6 (6000.3.11f1)
- **Language:** C#
- **Rendering:** Custom HLSL shaders for procedural celestial bodies, nebulae, and auroras
- **Platform:** iOS (iPhone + iPad), landscape orientation
- **Physics:** Custom 2D gravity model adapted from the original Apogee reference

## Project Structure

```
Assets/
  Scripts/
    CelestialBodies/   # Planet, Moon, CelestialBody base
    Rocket/            # RocketController, trajectory, exhaust
    Hazards/           # Asteroid, Satellite, AsteroidSpawner
    Levels/            # LevelConfig, LevelRegistry (45 levels)
    Rendering/         # Starfield, BodyPresets, procedural visuals
    Haptics/           # iOS native haptic feedback
    Input/             # Touch/drag/thrust input handling
    UI/                # MenuCarousel, StateOverlay, ScoreDisplay
    FX/                # Explosions, shooting stars
  Shaders/             # ProceduralBody shader
  Resources/Shaders/   # Nebula, Aurora shaders
  Plugins/iOS/         # Native haptic plugin
  Editor/              # Build scripts
```

## Building

### iOS Simulator (x86_64, requires Rosetta)

```bash
./build-ios.sh simulator
```

### iOS Device

```bash
APPLE_TEAM_ID=<your-team-id> ./build-ios.sh device
```

### Options

- `--skip-unity` — Skip Unity export, rebuild Xcode only
- `--clean` — Clean build directory first
- `--ipad` — Target iPad simulator

## Design

See [GAME_DESIGN.md](GAME_DESIGN.md) for the full game design document.
