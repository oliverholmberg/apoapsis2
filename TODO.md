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
- [ ] **Game intro / title screen** — neon arcade aesthetic, transitions to level selection
- [ ] **Level selection screen** — chapters with ~15 levels each, planet-colored spheres, high score, star rating, locked/greyed out states
- [ ] **Credits screen**

---

## Graphics & FX

- [x] **Crash explosion FX** — particle effect on rocket destruction
- [ ] **Rocket asset** — retro awesome rocket ship sprite, neon arcade aesthetic
- [x] **Rocket engine FX** — neon exhaust effect while thrusting
- [x] **Rocket trail** — fading neon trail following ship path, doubles as trajectory visual aid
- [ ] **Planet textures** — overlay textures on top of existing radial gradients

---

## Hazards

- [ ] **Satellites** — smaller bodies orbiting moons, no SOI, crash on contact, dynamic timing obstacle
- [ ] **Meteors / Asteroids** — random entry from sides and top, can achieve orbit around moons, destroyed on collision with each other or moons

---

## Level Framework

- [ ] **Configurable level system** — data-driven level definitions, easy to author new levels with options for moon config, SOI sizes, mass, hazard placement, coin layout, satellite config etc.
- [ ] **Sandbox / demo level** — designated staging level for testing all new components before integration into real levels. Used as the primary development playground.

---

## Audio

- [ ] **Music** — synth-heavy arcade track. Target Romaric Theme MIDI if recoverable, otherwise open-license synthwave alternative
- [ ] **Thrust SFX** — Apollo rocket engine audio (NASA public domain)
- [ ] **Lost in Space SFX** — NASA probe transmission audio (NASA public domain)
- [ ] **Crash SFX** — open source crash sound
- [ ] **Coin collect SFX** — open source collect sound
