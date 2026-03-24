# TODO_2.md
Ideas and tasks captured during mobile session. Merge into main TODO.md as appropriate.

---

## Workflow

- [ ] **GitHub streamlined workflow** — create `claude-notes` branch, protect `main` with PR requirement, scope personal access token to `apoapsis2` repo only. Claude pushes markdown to `claude-notes`, dev pulls into IDE. Revisit when ready.

---

## Menu System

- [x] **Menu system** — complete

---

## UI & UX

- [ ] **Rework level modals** — redesign win/crash/lost in space overlays
- [ ] **Chapter complete celebration** — special screen and effects when finishing all 15 levels in a chapter
- [ ] **Game complete celebration** — special screen and effects when finishing all chapters

---

## Gameplay & Level Design

- [x] **V1 target: 45 levels** — 3 chapters of 15 levels each
- [ ] **Haptics** — add haptic feedback for key game events: thrust start/stop, coin collect, crash, orbit achieved, level complete. Will significantly enhance game feel on mobile.
  - Use **Haptic Feedback for Unity** (formerly Lofelt Nice Vibrations) — free, open source, single API across iOS and Android, supports intensity and pattern control
  - Add early — retrofitting haptics later is painful
  - WebGL build will need haptics disabled gracefully

---

## Platform & Build

- [ ] **Headless terminal builds** — build and deploy without opening Xcode or Unity GUI
  - Unity headless build command via `-batchmode -executeMethod BuildScript.BuildIOS`
  - Requires `Assets/Editor/BuildScript.cs` — ask Claude Code to generate this
  - **Simulator:** Use `xcrun simctl` to boot, install, and launch on iOS Simulator entirely from terminal
  - **Physical device:** Use `xcodebuild` with device UDID (get via `xcrun xctrace list devices`) — still uses Xcode under the hood but no GUI
  - > **Note:** Fastlane wraps all of this into a single command — worth setting up once headless builds are working

- [ ] **iOS build & deploy via Fastlane** — CLI/CI approach, no Xcode GUI required
  - Install Fastlane: `brew install fastlane`
  - Run `fastlane init` in project root, configure a `Fastfile` with beta and release lanes
  - Use **Match** for certificate management — stores signing certs in a private Git repo, one source of truth
  - Write a Unity headless build script that Fastlane calls before packaging
  - Single command deploy to TestFlight: `fastlane ios beta`
  - > **Note:** Initial Fastlane + Match setup takes a few hours — Apple's provisioning system is fiddly. Zero maintenance once running.
  - > **Next step:** Wire into **GitHub Actions** for full CI/CD — push to `main`, GitHub builds Unity, Fastlane ships to TestFlight automatically
  - Still requires **Apple Developer Account** ($99/year for TestFlight and App Store)

---

## Procedural Visuals — Core Principle
> **Procedural visuals are the strong preference throughout this project.** All visuals should be generated via shaders, mesh generation, and particle systems where possible. External art assets should be the exception not the rule. **The rocket sprite is a deliberate exception — do not replace it.**

---

## Procedural Planet & Moon Visuals — ✅ COMPLETE

- [x] **Procedural planet/moon shader** — build in Unity Shader Graph. Each body gets a unique look from a single shared shader with exposed parameters per instance. Core components:
  - **Simplex/Perlin noise surface** — layered octaves for complex terrain variation. Domain warping for organic swirling gas giant patterns
  - **Animated surface drift** — slowly offset noise over time, makes bodies feel alive at zero performance cost
  - **Fresnel rim glow / atmosphere** — rim color contrasts with surface color, strongest visual upgrade available. Makes every sphere look like it has an atmosphere
  - **Emissive intensity** — planets and moons should glow against the dark background, not just reflect light

- [x] **Procedural planet/moon styles** — implement as parameter presets on the shared shader:
  - **Gas giant** — horizontal banded noise with slow drift and swirl vortex
  - **Lava world** — dark base with emissive glowing cracks in orange/red
  - **Ice/crystal world** — high contrast blue-white noise with specular highlights
  - **Cratered moon** — Voronoi noise for crater placement with normal map depth
  - **Neon marble** — swirling two-tone noise in bold neon colors with emissive rim

- [x] **Procedural star field backdrop** — generated star field with parallax depth layers, neon tinted, no static image

- [x] **Procedural planets & moons** — confirmed working, looks great
- [x] **Rocket sprite** — keeping existing sprite asset, works well against procedural planets. Consider adding neon bloom/glow post-processing around sprite to better integrate with procedural aesthetic if needed

- [x] **Procedural coin visuals** — glowing emissive geometry, satisfying pop FX on collect, all generated

- [x] **Procedural hazard visuals** — asteroids, satellites, and enemies all procedurally styled to read clearly as threats against the neon backdrop

---

## New Hazards & Complications

- [ ] **Additional hazards for later chapters** — introduce progressively to support 45 level arc. Ideas to evaluate:
  - Black holes — strong gravity well, instant death on contact
  - Moving planets — slow drifting gravitational bodies that shift SOI boundaries
  - Debris fields — clusters of small collision objects in transfer paths
  - Laser / energy barriers — timed or static beams blocking orbital paths
  - Gravity reversal zones — flip or reduce gravity in defined areas
  - Timed levels — complete all orbits before countdown expires
  - Orbital decay — orbits slowly decay and must be maintained with brief burns
  - Moon instability — moons slowly drift position mid-level

- [ ] **Enemy / predator ideas** — something that hunts the rocket to add pressure beyond static hazards. Ideas to evaluate:
  - Heat seeking missile — launches from moon surface after timer, follows rocket position, one speed, no orbital awareness
  - Turrets on moons — fixed gun emplacements, fire at intervals or when rocket enters arc, adds timing layer to orbit entry
  - Interceptor drone — dumb rocket, travels straight toward last known position, predictable but forces forward thinking
  - Patrol satellite — satellite with extended kill zone arm, forces precise orbit threading
  - Space jellyfish — slow random drifting creature, no targeting, organic unpredictable obstacle, strong flavor

- [ ] **Antagonist rocket (future)** — an AI opponent that navigates levels and executes orbital transfers. Genuinely hard AI problem. Recommended v1 approach if pursued: pre-recorded ghost replay of human playthrough. Full AI approaches include scripted burn sequences, genetic algorithm via Unity ML-Agents, or simplified cheat-based physics-aware opponent. Defer until core game is solid.
