# GAME_DESIGN.md

---

## 1. Overall Concept

A 2D mobile puzzle game based on orbital dynamics. Each level takes place 
in a small solar system viewed from the side. A planet sits at the bottom 
of the screen (roughly 15% visible), and the player pilots a rocket ship 
that launches from its surface.

The goal is to achieve stable orbits around every moon in the level — 
completing all orbits completes the level. Moons are orbited sequentially, 
with the player using orbital transfer maneuvers to hop between them.

Early levels fit entirely on screen. Later levels extend horizontally, 
requiring the player to pan left/right to survey the full layout before 
launching. The player chooses their first target moon by aligning the 
launch trajectory before blastoff.

**Controls are single-input:** the player holds the screen to fire the 
rocket's thruster. All trajectory and timing decisions flow from this one 
mechanic — when to burn, for how long, and from what point in the orbit. 
The simplicity of the input is the core design constraint.

Coins are scattered in rings around each moon and can be collected while 
in orbit or during well-timed transfer burns. Coin collection adds an 
optional skill layer on top of the core completion objective.

As levels progress, hazards such as asteroids are introduced, adding 
obstacles that must be avoided during burns and orbital paths.

**Core fantasy:** Feel like a real orbital mechanic — reading trajectories, 
timing burns with precision, and executing elegant multi-moon routes with 
a single finger.

**Note on physics:** The gravitational and orbital dynamics model is 
intentionally simplified to prioritize fun over realism. A working 2D 
gravitational body code sample is available and will be provided to Claude 
Code at development time. Do not implement a physics model from scratch — 
defer to the provided reference implementation.

---

## 2. Game Mechanics

### Thrust
The rocket thruster is binary — on or off, activated by holding the screen.
Thrust is instant with a single fixed power level. Fuel is unlimited in v1.

The ship always orients itself prograde — its nose continuously rotates to 
match the current velocity vector. This is automatic and not player 
controlled. This ensures that holding thrust always accelerates the ship 
along its current direction of travel, which is the key mechanic enabling 
orbital transfers with a single-input control scheme.

> **Note:** Auto-prograde orientation was a non-trivial implementation in 
> the original version. Reference carefully at dev time.

> **Future idea:** Introduce fuel limits in later levels as an additional 
> constraint and skill layer.

### Velocity Cap
The rocket has a hard velocity cap. Thrust has no effect beyond maximum 
speed. This keeps the physics environment predictable and the puzzle 
experience consistent.

### Gravity & Sphere of Influence (SOI)
The launch planet has no gravitational pull — it exists purely as a 
visual anchor and launch point. Only moons exert gravity.

Each moon has a defined Sphere of Influence (SOI). When the ship is within 
a moon's SOI, that moon's gravity is the dominant force acting on the ship. 
In early levels, SOIs are sized so they do not overlap. In later levels, 
SOIs may overlap slightly, creating complex gravitational transitions that 
increase transfer difficulty.

> **Note:** The gravity model is intentionally simplified. Reference 
> implementation will be provided at dev time — do not implement a physics 
> model from scratch.

### Orbit Detection
A stable orbit is detected using a quadrant system. The moon's SOI is 
divided into four quadrants. When the ship has passed through all four 
quadrants in sequence, a stable orbit is confirmed. This is simple, 
effective, and visually intuitive.

> **Future idea:** Explore alternative orbit detection methods if the 
> quadrant approach proves insufficient for edge cases.

### Orbital Transfers
Transfers between moons are organic and physics-driven. The player must 
thrust at the correct moment and duration to exit one SOI and enter another 
in a way that naturally leads to orbit. No guided assists or transfer 
indicators in v1 — the player reads the situation and burns.

### Failure States
- **Crashed** — the ship collides with a moon, planet, asteroid, satellite, 
  or other solid object.
- **Lost in Space** — the ship is thrust out of all SOIs with no 
  gravitational body to recapture it.

> Additional failure states may be introduced in later levels as new 
> hazards are added.

---

## 3. Game Objects & Behaviors

### Rocket
The player's ship. Thrusts in the prograde direction only (see Section 2).
Has a hard velocity cap — thrust has no effect beyond maximum speed.
Collision with any non-collectible object results in destruction ("Crashed").
The rocket has no gravity of its own.

### Planet
A large body anchored at the bottom of the screen (~15% visible). 
No gravitational pull. Purely a visual anchor and launch point, though it 
has a collision surface — crashing into it results in the standard 
"Crashed" failure state.

### Moons
The primary gravitational bodies and objectives of each level. Each moon 
has a defined Sphere of Influence (SOI) and a **mass** value that acts as 
a coefficient in the gravitational model, determining the strength of its 
pull and influencing orbital speed and SOI size. Mass and physical size can 
be tuned independently per level to create varied orbital challenges.

The player must achieve a stable orbit around every moon to complete the 
level. Moons vary in size across levels. They have a solid collision 
surface — contact results in "Crashed".

### Satellites
Smaller bodies that orbit around moons using the same gravitational model. 
Purely obstacles — they have no SOI and cannot be orbited. Contact results 
in "Crashed". Their orbital motion creates dynamic timing challenges for 
orbit entry and transfer maneuvers.

### Coins
Collectibles scattered around moons. In early levels arranged in a clean 
circle representing the ideal orbit path, serving as a visual guide. In 
later levels placement becomes more irregular to increase difficulty and 
reward planning. Collected by flying through them. A subtle magnet effect 
draws nearby coins toward the ship — radius should be tunable for balancing. 
Coin collection is optional but scored.

### Asteroids
Hazards that enter levels randomly from the sides and top of the screen. 
Move along unpredictable paths. Capable of falling into a moon's SOI and 
achieving orbit, becoming persistent dynamic obstacles. Asteroids that 
collide with each other or with a moon are destroyed. Contact with the 
rocket results in "Crashed".

> **Future ideas:** Additional hazards to explore in later levels —
> lightning strikes, extreme cold, extreme heat.

---

## 4. Visual & Audio Style

### Visual Style
Neon arcade aesthetic — visually loud, vibrant, and high energy. 
Dark space backdrop with glowing neon elements. Everything should 
feel punchy and alive.

- **Backdrop:** Deep space — dark background with neon star fields. 
  Real space photography may be used as a base layer but heavily 
  stylized with neon overlay effects.
- **Planet:** Radial gradient, bright neon colors. Glowing rim effect.
- **Moons:** Radial gradients, each with a distinct bright neon color 
  palette to differentiate them at a glance. Glow effects on SOI boundary 
  (subtle, not intrusive).
- **Rocket:** Retro-styled, iconic silhouette — think classic sci-fi 
  rocket with a neon arcade treatment. Should feel awesome.
- **Coins:** Bright, glowing collectibles — satisfying visual pop on 
  collection.
- **Asteroids & Satellites:** Darker, rougher aesthetic to contrast 
  with the glowing objectives — clearly readable as hazards.
- **Thrust FX:** Neon exhaust trail that follows the ship's path, 
  fading over time. Doubles as a trajectory visual aid.

> **Future idea:** Upgradable ship skins with increasingly elaborate 
> retro-awesome designs.

### Audio

**Music**
The original soundtrack used "Romaric Theme" (public domain MIDI) 
processed through hardware synths — a great fit for the neon arcade 
aesthetic. The original mix is lost but the MIDI file may be 
recoverable. Target a similar synth-heavy, energetic feel for v1.
Explore open-license synthwave or arcade-style tracks as alternatives 
if needed.

**Sound Effects**
The original used NASA Apollo mission audio (public domain) to great effect:
- **Thrust:** Apollo rocket engine audio
- **Lost in Space:** Old NASA probe transmission audio
- **Crashed / Coin collect:** Open source SFX

These sources are public domain and worth tracking down again. 
NASA's audio archive is publicly available at nasa.gov.

---

## 5. UI & UX Flow

### Title Screen
Neon arcade aesthetic consistent with gameplay visuals — dark background, 
glowing star field, neon typography. Transitions to Level Selection 
automatically or on tap.

### Level Selection
Levels are grouped into chapters. V1 targets multiple chapters with 
~15 levels each. Levels are displayed as spheres colored to match their 
planet, with high score and star rating displayed beneath each.

- Completed levels show score and stars earned
- Unplayed levels are greyed out and locked
- Chapters may scroll horizontally or be paginated

> **Star rating system:** 3-star rating per level based on performance 
> (coins collected, score, or time — to be defined in progression section).

### Level Transition
When a level is selected or completed, the screen rapidly rotates into 
the next level — a signature animation consistent with the orbital theme.

### Gameplay HUD
Minimal by design — a single point score counter visible during play. 
Nothing else on screen to distract from orbit reading and timing.

### Pause Menu
Accessible during gameplay. Options:
- Resume
- Restart Level
- Quit to Level Selection

### Level Complete Screen
Displays end-of-level summary:
- Score achieved
- Stars earned
- Coins collected
- Next level button
- Replay button

### Failure Screen
Triggered on "Crashed" or "Lost in Space" events. Displays the 
relevant failure message with options to:
- Retry
- Quit to Level Selection

---

## 6. Progression & Difficulty

Difficulty scales across levels and chapters through the following levers:

### Level Layout Complexity
- **Early levels:** Few moons, all on screen, no SOI overlap, 
  coins arranged in ideal orbit circles as visual guides.
- **Later levels:** More moons, wider layouts requiring left/right 
  panning, moons spaced further apart making transfers more demanding, 
  SOIs may overlap slightly creating complex gravitational transitions.

### Moon Arrangements
Some levels feature optional moon configurations that are not required 
for completion but reward high scores for players who attempt them. 
These add a skill ceiling beyond basic level completion.

### Hazards
Introduced progressively across chapters:
- **Asteroids** — random entry from sides and top, capable of 
  achieving orbit around moons and becoming persistent obstacles.
- **Satellites** — orbiting sub-bodies around moons, creating 
  dynamic timing challenges for orbit entry and transfers.

> **Future ideas:** Additional hazards and challenges to be designed 
> for later chapters — lightning strikes, extreme cold, extreme heat, 
> and others TBD.

### Star Rating
Star ratings per level provide a soft progression layer — players can 
complete a level with one star and return later for a perfect run. 
Rating criteria TBD (likely based on coins collected, score, and/or 
efficiency of burns).

---

## 7. Platform & Technical Notes

### Target Platforms
- iOS
- Android
- Web (primary testing/iteration platform during development)

### Orientation
Landscape only.

### Engine
Unity is the current candidate based on prior experience with the 
original version. Alternatives have not been ruled out.

> **Decision needed:** Final engine selection before development begins.

### Input
Single touch — one finger hold to thrust. No multitouch required in v1.
Touch input should be consistent across iOS, Android, and web builds.

### Physics
The gravitational and orbital dynamics model is intentionally simplified.
A reference implementation from the original codebase is available and 
will be provided to Claude Code at development time. Do not implement 
a physics model from scratch.

### Device Support
Minimum OS versions and device constraints TBD.

---

## 8. Out of Scope (v1)

The following ideas are explicitly deferred to future versions. 
They should not be implemented in v1 but are noted here for future 
development consideration.

### Gameplay
- Fuel limits as a late-level constraint
- Additional failure states beyond Crashed and Lost in Space
- Lightning strikes, extreme cold, extreme heat hazards
- Additional hazard types TBD

### Progression
- Star rating criteria (coins, score, burn efficiency) — TBD
- Optional moon arrangements for high score — layout TBD per level

### Visual
- Upgradable ship skins with retro-awesome designs

### Audio
- Reproduce original Romaric Theme synth mix (MIDI may be recoverable)
- Reproduce original NASA Apollo audio mix

### Platform
- Minimum OS version and device support constraints
- Final engine selection (Unity vs alternatives)
