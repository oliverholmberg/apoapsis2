# TODO.md
Development task list for Apoapsis 2. Check off items as they are completed so Claude Code can track progress.

---

## Gameplay

- [x] **Orbital transfer tuning** — transfers are too difficult even on early levels. Tune gravity coefficients, SOI sizes, and velocity cap to make first-moon transfers more forgiving. Iterate on sandbox level.

---

## UI & UX

- [x] **Win state screen** — display on level complete with score, stars, coins collected, next level and replay buttons
- [x] **Crash state screen** — display "Crashed" message with retry and quit options
- [x] **Lost in Space state screen** — display "Lost in Space" message with retry and quit options
- [x] **Pause menu** — resume, restart level, quit to level selection
- [x] **Restart** — restart current level from pause menu and failure screens
- [x] **Game intro / title screen** — neon arcade aesthetic, transitions to level selection
- [x] **Level selection screen** — chapters with ~15 levels each, planet-colored spheres, high score, star rating, locked/greyed out states
- [ ] **Credits screen**

---

## Graphics & FX

- [x] **Crash explosion FX** — particle effect on rocket destruction
- [x] **Rocket asset** — rocket sprite with neon arcade aesthetic
- [x] **Rocket engine FX** — neon exhaust effect while thrusting
- [x] **Rocket trail** — fading neon trail following ship path, doubles as trajectory visual aid
- [x] **Planet textures** — procedural shader with 10 body styles
- [x] **Procedural coin visuals** — glowing emissive geometry with pop FX on collect
- [x] **Procedural hazard visuals** — asteroids and satellites procedurally styled
- [x] **Rocket neon bloom/glow** — integrated with procedural aesthetic

---

## Hazards

- [x] **Satellites** — smaller bodies orbiting moons, no SOI, crash on contact, dynamic timing obstacle
- [x] **Meteors / Asteroids** — random entry from sides and top, can achieve orbit around moons, destroyed on collision with each other or moons
- [ ] **Additional hazards for later chapters** — black holes, moving planets, debris fields, laser barriers, gravity reversal zones, timed levels, orbital decay, moon instability
- [ ] **Enemy / predator ideas** — heat seeking missiles, turrets, interceptor drones, patrol satellites, space jellyfish
- [ ] **Antagonist rocket (future)** — AI opponent or ghost replay

---

## Level Framework

- [x] **Configurable level system** — data-driven level definitions, easy to author new levels
- [x] **V1 target: 45 levels** — 3 chapters of 15 levels each
- [x] **Sandbox / demo level** — designated staging level for testing all new components

---

## Audio

- [ ] **Music** — synth-heavy arcade track
- [ ] **Thrust SFX** — Apollo rocket engine audio (NASA public domain)
- [ ] **Lost in Space SFX** — NASA probe transmission audio (NASA public domain)
- [ ] **Crash SFX** — open source crash sound
- [ ] **Coin collect SFX** — open source collect sound

---

## Mobile & Platform

- [ ] **Haptics** — haptic feedback for thrust, coin collect, crash, orbit achieved, level complete
- [ ] **Headless terminal builds** — build and deploy without opening Xcode or Unity GUI
- [ ] **iOS build & deploy via Fastlane** — CLI/CI approach
- [ ] **GitHub streamlined workflow** — claude-notes branch, protected main, scoped PAT
