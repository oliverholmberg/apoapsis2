using UnityEngine;

public static class LevelRegistry
{
    public static int CurrentChapter = 1;
    public static int CurrentLevel = 1;

    // Helper to reduce boilerplate
    static LevelConfig MakeLevel(
        Color planetCore, Color planetRim, BodyStyle planetStyle, Color planetAtmo,
        MoonConfig[] moons,
        bool asteroids = false, float asteroidInterval = 5f,
        float thrust = 8f, float maxSpd = 8f, float launch = 3f,
        float ortho = 10f, float lostDist = 25f,
        int star1 = 100, int star2 = 200, int star3 = 350)
    {
        return new LevelConfig
        {
            planet = new PlanetConfig
            {
                radius = 5f,
                coreColor = planetCore,
                rimColor = planetRim,
                style = planetStyle,
                atmosphereColor = planetAtmo
            },
            moons = moons,
            rocket = new RocketConfig { thrustForce = thrust, maxSpeed = maxSpd, launchSpeed = launch, brakeFactor = 3f },
            asteroids = new AsteroidConfig { enabled = asteroids, spawnInterval = asteroidInterval },
            camera = new CameraConfig { orthoSize = ortho, position = new Vector2(0f, 5f) },
            lostInSpaceDistance = lostDist,
            starThresholds = new[] { star1, star2, star3 }
        };
    }

    static MoonConfig Moon(Vector2 pos, float r, float mass, int soi, Color core, Color rim,
        BodyStyle style = BodyStyle.CrateredMoon, Color? atmo = null,
        int coins = 12, float coinR = 2.5f, SatelliteConfig[] sats = null)
    {
        return new MoonConfig
        {
            position = pos, radius = r, mass = mass, soiRadius = soi,
            coreColor = core, rimColor = rim,
            style = style, atmosphereColor = atmo ?? rim,
            coinCount = coins, coinOrbitRadius = coinR, satellites = sats
        };
    }

    static SatelliteConfig Sat(float orbitR = 2.5f, float angle = 0f, float r = 0.2f,
        Color? core = null, Color? rim = null, BodyStyle style = BodyStyle.CrateredMoon, Color? atmo = null)
    {
        var c = core ?? new Color(1f, 0.4f, 0f);
        var ri = rim ?? new Color(1f, 0.8f, 0.2f);
        return new SatelliteConfig
        {
            orbitRadius = orbitR, startAngle = angle, radius = r,
            coreColor = c, rimColor = ri, style = style,
            atmosphereColor = atmo ?? ri
        };
    }

    // Chapter 1: Learning the basics (15 levels)
    static readonly LevelConfig[] Chapter1 = new[]
    {
        // 1-1: Single moon, easy orbit — neon marble planet
        MakeLevel(
            new Color(0.1f, 0f, 0.4f), new Color(1f, 0f, 0.8f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 1f),
            new[] { Moon(new Vector2(2f, 14f), 1f, 0.05f, 5, new Color(0f, 0.2f, 0.4f), new Color(0f, 1f, 1f), BodyStyle.CrateredMoon, new Color(0.3f, 0.8f, 1f)) },
            star1: 80, star2: 150, star3: 250
        ),

        // 1-2: Single moon, tighter orbit — ice planet
        MakeLevel(
            new Color(0.05f, 0.1f, 0.3f), new Color(0.2f, 0.5f, 1f), BodyStyle.IceCrystal, new Color(0.4f, 0.6f, 1f),
            new[] { Moon(new Vector2(-1f, 13f), 0.8f, 0.04f, 4, new Color(0.1f, 0.1f, 0.3f), new Color(0.4f, 0.4f, 1f), BodyStyle.CrateredMoon, new Color(0.5f, 0.5f, 1f)) },
            star1: 80, star2: 160, star3: 260
        ),

        // 1-3: Two moons, intro to orbital transfer — lava planet
        MakeLevel(
            new Color(0.2f, 0f, 0.1f), new Color(1f, 0.3f, 0.4f), BodyStyle.LavaWorld, new Color(1f, 0.4f, 0.2f),
            new[] {
                Moon(new Vector2(2f, 14f), 1f, 0.05f, 5, new Color(0f, 0.2f, 0.4f), new Color(0f, 1f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.9f, 1f)),
                Moon(new Vector2(-8f, 11f), 0.8f, 0.05f, 5, new Color(0.3f, 0f, 0.3f), new Color(1f, 0.4f, 1f), BodyStyle.NeonMarble, new Color(1f, 0.5f, 1f))
            },
            star1: 100, star2: 200, star3: 350
        ),

        // 1-4: Two moons, wider spread — oceanic green
        MakeLevel(
            new Color(0f, 0.15f, 0.1f), new Color(0f, 1f, 0.6f), BodyStyle.Oceanic, new Color(0.2f, 1f, 0.7f),
            new[] {
                Moon(new Vector2(5f, 13f), 1f, 0.05f, 5, new Color(0.2f, 0.15f, 0f), new Color(1f, 0.8f, 0.2f), BodyStyle.LavaWorld, new Color(1f, 0.6f, 0.1f)),
                Moon(new Vector2(-6f, 15f), 0.9f, 0.05f, 5, new Color(0f, 0.2f, 0.1f), new Color(0.3f, 1f, 0.5f), BodyStyle.CrateredMoon, new Color(0.4f, 1f, 0.6f))
            },
            star1: 100, star2: 220, star3: 370
        ),

        // 1-5: Single moon + satellite — orange neon marble
        MakeLevel(
            new Color(0.15f, 0.05f, 0f), new Color(1f, 0.5f, 0f), BodyStyle.NeonMarble, new Color(1f, 0.6f, 0.2f),
            new[] {
                Moon(new Vector2(1f, 13f), 1.1f, 0.05f, 5, new Color(0.15f, 0.1f, 0.3f), new Color(0.6f, 0.4f, 1f), BodyStyle.NeonMarble, new Color(0.7f, 0.5f, 1f),
                    sats: new[] { Sat(2.5f, 0f, core: new Color(0.3f, 0f, 0.3f), rim: new Color(1f, 0.3f, 1f), style: BodyStyle.NeonMarble, atmo: new Color(1f, 0.4f, 1f)) })
            },
            star1: 100, star2: 200, star3: 320
        ),

        // 1-6: Two moons + satellite — purple gas giant
        MakeLevel(
            new Color(0.1f, 0f, 0.2f), new Color(0.7f, 0.2f, 1f), BodyStyle.GasGiant, new Color(0.8f, 0.3f, 1f),
            new[] {
                Moon(new Vector2(3f, 14f), 1f, 0.05f, 5, new Color(0.1f, 0.2f, 0f), new Color(0.5f, 1f, 0f), BodyStyle.CrateredMoon, new Color(0.6f, 1f, 0.3f)),
                Moon(new Vector2(-7f, 10f), 0.9f, 0.05f, 5, new Color(0.2f, 0f, 0.1f), new Color(1f, 0.2f, 0.5f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.4f),
                    sats: new[] { Sat(2.5f, 90f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.2f, 0.1f), style: BodyStyle.LavaWorld, atmo: new Color(1f, 0.3f, 0.1f)) })
            },
            star1: 110, star2: 230, star3: 380
        ),

        // 1-7: Two moons, asteroids — storm planet
        MakeLevel(
            new Color(0.05f, 0.05f, 0.15f), new Color(0.4f, 0.6f, 1f), BodyStyle.Storm, new Color(0.5f, 0.7f, 1f),
            new[] {
                Moon(new Vector2(0f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0.1f, 0f), new Color(1f, 0.6f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.5f, 0f)),
                Moon(new Vector2(-9f, 12f), 0.8f, 0.05f, 5, new Color(0f, 0.15f, 0.2f), new Color(0f, 0.8f, 1f), BodyStyle.IceCrystal, new Color(0.2f, 0.8f, 1f))
            },
            asteroids: true, asteroidInterval: 8f,
            star1: 120, star2: 250, star3: 400
        ),

        // 1-8: Three moons — dark neon green
        MakeLevel(
            new Color(0f, 0.1f, 0f), new Color(0f, 0.8f, 0.1f), BodyStyle.NeonMarble, new Color(0f, 1f, 0.2f),
            new[] {
                Moon(new Vector2(2f, 13f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.3f), new Color(0.2f, 0.6f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.7f, 1f)),
                Moon(new Vector2(-7f, 15f), 0.8f, 0.04f, 4, new Color(0.15f, 0f, 0.2f), new Color(0.8f, 0.3f, 1f), BodyStyle.NeonMarble, new Color(0.9f, 0.4f, 1f)),
                Moon(new Vector2(8f, 17f), 0.7f, 0.04f, 4, new Color(0.2f, 0.2f, 0f), new Color(1f, 1f, 0.3f), BodyStyle.CrateredMoon, new Color(1f, 1f, 0.5f))
            },
            ortho: 12f, lostDist: 30f,
            star1: 150, star2: 300, star3: 480
        ),

        // 1-9: Two moons, both with satellites — neon red lava
        MakeLevel(
            new Color(0.25f, 0f, 0f), new Color(1f, 0.1f, 0.1f), BodyStyle.NeonMarble, new Color(1f, 0.2f, 0.15f),
            new[] {
                Moon(new Vector2(3f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.3f, 0.2f), BodyStyle.LavaWorld, new Color(1f, 0.4f, 0.2f),
                    sats: new[] { Sat(2.5f, 45f, core: new Color(0f, 0.1f, 0.2f), rim: new Color(0.2f, 0.8f, 1f), style: BodyStyle.IceCrystal, atmo: new Color(0.3f, 0.9f, 1f)) }),
                Moon(new Vector2(-6f, 11f), 0.9f, 0.05f, 5, new Color(0f, 0.2f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.CrateredMoon, new Color(0.3f, 1f, 0.4f),
                    sats: new[] { Sat(2.5f, 180f, core: new Color(0.15f, 0.1f, 0f), rim: new Color(1f, 0.7f, 0f), style: BodyStyle.LavaWorld, atmo: new Color(1f, 0.8f, 0.2f)) })
            },
            star1: 130, star2: 260, star3: 420
        ),

        // 1-10: Single large moon, asteroids — crystalline planet
        MakeLevel(
            new Color(0.15f, 0f, 0.15f), new Color(0.9f, 0.1f, 0.9f), BodyStyle.Crystalline, new Color(1f, 0.3f, 1f),
            new[] {
                Moon(new Vector2(0f, 12f), 1.5f, 0.06f, 6, new Color(0.1f, 0.1f, 0.1f), new Color(0.7f, 0.7f, 0.8f), BodyStyle.Crystalline, new Color(0.8f, 0.8f, 0.9f),
                    coins: 16, coinR: 3f)
            },
            asteroids: true, asteroidInterval: 6f,
            star1: 120, star2: 240, star3: 380
        ),

        // 1-11: Three moons + satellite — gold gas giant
        MakeLevel(
            new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.9f, 0.2f), BodyStyle.GasGiant, new Color(1f, 1f, 0.4f),
            new[] {
                Moon(new Vector2(1f, 13f), 0.9f, 0.05f, 5, new Color(0f, 0f, 0.2f), new Color(0.3f, 0.3f, 1f), BodyStyle.IceCrystal, new Color(0.4f, 0.4f, 1f)),
                Moon(new Vector2(-8f, 16f), 0.8f, 0.04f, 4, new Color(0.2f, 0.1f, 0f), new Color(1f, 0.5f, 0f), BodyStyle.LavaWorld, new Color(1f, 0.6f, 0.1f),
                    sats: new[] { Sat(2f, 0f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.3f, 1f, 0.4f), style: BodyStyle.NeonMarble, atmo: new Color(0.4f, 1f, 0.5f)) }),
                Moon(new Vector2(7f, 18f), 0.7f, 0.04f, 4, new Color(0f, 0.2f, 0.15f), new Color(0.2f, 1f, 0.7f), BodyStyle.NeonMarble, new Color(0.3f, 1f, 0.8f))
            },
            ortho: 12f, lostDist: 32f,
            star1: 160, star2: 320, star3: 500
        ),

        // 1-12: Two moons, asteroids + satellite — toxic planet
        MakeLevel(
            new Color(0.05f, 0.1f, 0f), new Color(0.4f, 1f, 0.2f), BodyStyle.Toxic, new Color(0.5f, 1f, 0.3f),
            new[] {
                Moon(new Vector2(4f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0.05f, 0f), new Color(1f, 0.3f, 0f), BodyStyle.LavaWorld, new Color(1f, 0.4f, 0f)),
                Moon(new Vector2(-5f, 10f), 1f, 0.05f, 5, new Color(0f, 0.15f, 0.1f), new Color(0f, 0.9f, 0.5f), BodyStyle.CrateredMoon, new Color(0.2f, 1f, 0.6f),
                    sats: new[] { Sat(3f, 120f, core: new Color(0.1f, 0f, 0.15f), rim: new Color(0.7f, 0.2f, 1f), style: BodyStyle.GasGiant, atmo: new Color(0.8f, 0.3f, 1f)) })
            },
            asteroids: true, asteroidInterval: 5f,
            star1: 130, star2: 270, star3: 430
        ),

        // 1-13: Three moons, two with satellites — mountainous amber
        MakeLevel(
            new Color(0.15f, 0.1f, 0f), new Color(1f, 0.7f, 0.2f), BodyStyle.Mountainous, new Color(1f, 0.8f, 0.3f),
            new[] {
                Moon(new Vector2(2f, 12f), 1f, 0.05f, 5, new Color(0f, 0.1f, 0.2f), new Color(0.1f, 0.6f, 1f), BodyStyle.Mountainous, new Color(0.2f, 0.7f, 1f),
                    sats: new[] { Sat(2.5f, 30f, core: new Color(0.2f, 0.1f, 0f), rim: new Color(1f, 0.6f, 0f), style: BodyStyle.CrateredMoon, atmo: new Color(1f, 0.7f, 0.2f)) }),
                Moon(new Vector2(-7f, 15f), 0.8f, 0.05f, 5, new Color(0.15f, 0f, 0.1f), new Color(0.9f, 0.2f, 0.5f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 0.6f),
                    sats: new[] { Sat(2.5f, 210f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0f, 0.9f, 0.8f), style: BodyStyle.IceCrystal, atmo: new Color(0.2f, 1f, 0.9f)) }),
                Moon(new Vector2(6f, 19f), 0.7f, 0.04f, 4, new Color(0.1f, 0.15f, 0f), new Color(0.6f, 1f, 0f), BodyStyle.CrateredMoon, new Color(0.7f, 1f, 0.2f))
            },
            ortho: 12f, lostDist: 32f,
            star1: 170, star2: 340, star3: 540
        ),

        // 1-14: Three moons + asteroids — mauve gas giant
        MakeLevel(
            new Color(0.1f, 0f, 0.1f), new Color(0.8f, 0.2f, 0.6f), BodyStyle.GasGiant, new Color(0.9f, 0.3f, 0.7f),
            new[] {
                Moon(new Vector2(0f, 13f), 1f, 0.05f, 5, new Color(0.15f, 0.15f, 0f), new Color(1f, 1f, 0.4f), BodyStyle.LavaWorld, new Color(1f, 1f, 0.5f)),
                Moon(new Vector2(-9f, 11f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.15f), new Color(0.2f, 0.7f, 0.9f), BodyStyle.IceCrystal, new Color(0.3f, 0.8f, 1f),
                    sats: new[] { Sat(2.5f, 60f, core: new Color(0.15f, 0.15f, 0f), rim: new Color(1f, 1f, 0.3f), style: BodyStyle.NeonMarble, atmo: new Color(1f, 1f, 0.5f)) }),
                Moon(new Vector2(7f, 17f), 0.8f, 0.04f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.CrateredMoon, new Color(1f, 0.3f, 0.2f))
            },
            asteroids: true, asteroidInterval: 5f,
            ortho: 12f, lostDist: 32f,
            star1: 180, star2: 360, star3: 560
        ),

        // 1-15: BOSS — white neon marble, everything
        MakeLevel(
            new Color(0.05f, 0.05f, 0.05f), new Color(0.9f, 0.9f, 0.9f), BodyStyle.NeonMarble, new Color(0.8f, 0.8f, 1f),
            new[] {
                Moon(new Vector2(3f, 13f), 1.1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.15f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f),
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(3f, 0f, core: new Color(0.2f, 0.1f, 0f), rim: new Color(1f, 0.5f, 0f), style: BodyStyle.LavaWorld, atmo: new Color(1f, 0.6f, 0.1f)) }),
                Moon(new Vector2(-8f, 16f), 0.9f, 0.05f, 5, new Color(0f, 0f, 0.2f), new Color(0.2f, 0.3f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.4f, 1f),
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(2.5f, 90f, core: new Color(0.1f, 0f, 0.15f), rim: new Color(0.6f, 0.2f, 1f), style: BodyStyle.GasGiant, atmo: new Color(0.7f, 0.3f, 1f)) }),
                Moon(new Vector2(6f, 20f), 0.8f, 0.05f, 5, new Color(0f, 0.2f, 0f), new Color(0.1f, 1f, 0.2f), BodyStyle.NeonMarble, new Color(0.2f, 1f, 0.3f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 180f, core: new Color(0f, 0.15f, 0.1f), rim: new Color(0f, 1f, 0.6f), style: BodyStyle.IceCrystal, atmo: new Color(0.2f, 1f, 0.7f)) })
            },
            asteroids: true, asteroidInterval: 4f,
            ortho: 13f, lostDist: 35f,
            star1: 200, star2: 400, star3: 620
        )
    };

    // Chapter 2: Intermediate — tighter orbits, more hazards (15 levels)
    static readonly LevelConfig[] Chapter2 = new[]
    {
        // 2-1: Single moon, tight SOI — oceanic planet
        MakeLevel(
            new Color(0f, 0.1f, 0.2f), new Color(0f, 0.6f, 1f), BodyStyle.Oceanic, new Color(0.2f, 0.7f, 1f),
            new[] { Moon(new Vector2(1f, 13f), 0.9f, 0.05f, 4, new Color(0.1f, 0.1f, 0f), new Color(0.8f, 0.8f, 0.2f), BodyStyle.Mountainous, new Color(0.9f, 0.9f, 0.3f)) },
            thrust: 7f, maxSpd: 7f, star1: 100, star2: 200, star3: 320
        ),
        // 2-2: Two moons, close together — storm planet
        MakeLevel(
            new Color(0.1f, 0.05f, 0.15f), new Color(0.6f, 0.3f, 1f), BodyStyle.Storm, new Color(0.7f, 0.4f, 1f),
            new[] {
                Moon(new Vector2(3f, 12f), 0.8f, 0.05f, 4, new Color(0f, 0.15f, 0.1f), new Color(0f, 1f, 0.5f), BodyStyle.Toxic, new Color(0.2f, 1f, 0.6f)),
                Moon(new Vector2(-4f, 15f), 0.8f, 0.05f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f))
            },
            star1: 120, star2: 240, star3: 380
        ),
        // 2-3: Single moon + 2 satellites — crystalline planet
        MakeLevel(
            new Color(0.1f, 0.05f, 0.15f), new Color(0.7f, 0.5f, 1f), BodyStyle.Crystalline, new Color(0.8f, 0.6f, 1f),
            new[] {
                Moon(new Vector2(0f, 13f), 1f, 0.05f, 5, new Color(0f, 0.1f, 0.2f), new Color(0.2f, 0.7f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.8f, 1f),
                    sats: new[] { Sat(2.5f, 0f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.3f, 0.1f), style: BodyStyle.LavaWorld), Sat(2.5f, 180f, core: new Color(0f, 0.2f, 0f), rim: new Color(0.3f, 1f, 0.3f), style: BodyStyle.Toxic) })
            },
            star1: 120, star2: 250, star3: 400
        ),
        // 2-4: Two moons, asteroids, tight — toxic planet
        MakeLevel(
            new Color(0.05f, 0.1f, 0f), new Color(0.3f, 0.9f, 0.1f), BodyStyle.Toxic, new Color(0.4f, 1f, 0.2f),
            new[] {
                Moon(new Vector2(4f, 13f), 0.9f, 0.05f, 4, new Color(0.15f, 0.1f, 0f), new Color(1f, 0.7f, 0f), BodyStyle.Mountainous, new Color(1f, 0.8f, 0.2f)),
                Moon(new Vector2(-5f, 10f), 0.9f, 0.05f, 4, new Color(0f, 0f, 0.2f), new Color(0.3f, 0.3f, 1f), BodyStyle.Crystalline, new Color(0.4f, 0.4f, 1f))
            },
            asteroids: true, asteroidInterval: 6f,
            star1: 130, star2: 260, star3: 420
        ),
        // 2-5: Three moons, satellites — mountainous planet
        MakeLevel(
            new Color(0.1f, 0.08f, 0.02f), new Color(0.7f, 0.5f, 0.2f), BodyStyle.Mountainous, new Color(0.8f, 0.6f, 0.3f),
            new[] {
                Moon(new Vector2(2f, 12f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.15f), new Color(0.1f, 0.7f, 0.9f), BodyStyle.Oceanic, new Color(0.2f, 0.8f, 1f),
                    sats: new[] { Sat(2.5f, 45f, core: new Color(0.15f, 0f, 0.15f), rim: new Color(0.8f, 0.2f, 0.8f), style: BodyStyle.NeonMarble) }),
                Moon(new Vector2(-7f, 15f), 0.8f, 0.04f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.2f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.2f)),
                Moon(new Vector2(6f, 18f), 0.7f, 0.04f, 4, new Color(0f, 0.15f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.Toxic, new Color(0.3f, 1f, 0.4f))
            },
            ortho: 12f, lostDist: 30f,
            star1: 160, star2: 320, star3: 500
        ),
        // 2-6: Two moons, heavy asteroids — neon marble
        MakeLevel(
            new Color(0.15f, 0f, 0.2f), new Color(1f, 0.2f, 0.8f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 0.9f),
            new[] {
                Moon(new Vector2(3f, 14f), 1f, 0.05f, 5, new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.9f, 0.2f), BodyStyle.Storm, new Color(1f, 1f, 0.3f)),
                Moon(new Vector2(-6f, 11f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.2f), new Color(0f, 0.8f, 1f), BodyStyle.IceCrystal, new Color(0.2f, 0.9f, 1f))
            },
            asteroids: true, asteroidInterval: 4f,
            star1: 140, star2: 280, star3: 440
        ),
        // 2-7: Single moon, 3 satellites — gas giant
        MakeLevel(
            new Color(0.1f, 0.05f, 0f), new Color(0.9f, 0.6f, 0.1f), BodyStyle.GasGiant, new Color(1f, 0.7f, 0.2f),
            new[] {
                Moon(new Vector2(0f, 13f), 1.2f, 0.06f, 6, new Color(0.05f, 0.1f, 0.15f), new Color(0.3f, 0.7f, 1f), BodyStyle.Oceanic, new Color(0.4f, 0.8f, 1f),
                    coins: 16, coinR: 3f,
                    sats: new[] {
                        Sat(3f, 0f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.2f, 0.1f), style: BodyStyle.LavaWorld),
                        Sat(3f, 120f, core: new Color(0f, 0.15f, 0f), rim: new Color(0f, 0.9f, 0.3f), style: BodyStyle.Toxic),
                        Sat(3f, 240f, core: new Color(0.1f, 0f, 0.15f), rim: new Color(0.6f, 0.2f, 1f), style: BodyStyle.Crystalline)
                    })
            },
            star1: 140, star2: 280, star3: 450
        ),
        // 2-8: Three moons, asteroids — ice crystal
        MakeLevel(
            new Color(0.02f, 0.05f, 0.15f), new Color(0.2f, 0.4f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.5f, 1f),
            new[] {
                Moon(new Vector2(1f, 12f), 0.9f, 0.05f, 5, new Color(0.2f, 0.05f, 0f), new Color(1f, 0.4f, 0f), BodyStyle.LavaWorld, new Color(1f, 0.5f, 0.1f)),
                Moon(new Vector2(-8f, 14f), 0.8f, 0.04f, 4, new Color(0f, 0.15f, 0.1f), new Color(0.2f, 1f, 0.6f), BodyStyle.NeonMarble, new Color(0.3f, 1f, 0.7f)),
                Moon(new Vector2(7f, 17f), 0.7f, 0.04f, 4, new Color(0.1f, 0.1f, 0f), new Color(0.8f, 0.8f, 0.1f), BodyStyle.Mountainous, new Color(0.9f, 0.9f, 0.3f))
            },
            asteroids: true, asteroidInterval: 5f,
            ortho: 12f, lostDist: 30f,
            star1: 170, star2: 340, star3: 540
        ),
        // 2-9: Two moons, both with satellites — storm planet
        MakeLevel(
            new Color(0.08f, 0.05f, 0.12f), new Color(0.5f, 0.3f, 0.9f), BodyStyle.Storm, new Color(0.6f, 0.4f, 1f),
            new[] {
                Moon(new Vector2(4f, 13f), 1f, 0.05f, 5, new Color(0f, 0.2f, 0.1f), new Color(0f, 1f, 0.5f), BodyStyle.Oceanic, new Color(0.2f, 1f, 0.6f),
                    sats: new[] { Sat(2.5f, 30f, core: new Color(0.15f, 0.15f, 0f), rim: new Color(1f, 1f, 0.2f), style: BodyStyle.Crystalline) }),
                Moon(new Vector2(-5f, 10f), 0.9f, 0.05f, 5, new Color(0.2f, 0f, 0.1f), new Color(1f, 0.2f, 0.5f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 0.6f),
                    sats: new[] { Sat(2.5f, 210f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0f, 0.8f, 0.8f), style: BodyStyle.IceCrystal) })
            },
            star1: 150, star2: 300, star3: 480
        ),
        // 2-10: Three moons, satellites + asteroids — toxic
        MakeLevel(
            new Color(0.05f, 0.12f, 0f), new Color(0.4f, 1f, 0.1f), BodyStyle.Toxic, new Color(0.5f, 1f, 0.2f),
            new[] {
                Moon(new Vector2(2f, 12f), 1f, 0.05f, 5, new Color(0.1f, 0f, 0.15f), new Color(0.7f, 0.2f, 1f), BodyStyle.GasGiant, new Color(0.8f, 0.3f, 1f),
                    sats: new[] { Sat(2.5f, 60f, core: new Color(0.2f, 0.1f, 0f), rim: new Color(1f, 0.6f, 0f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(-7f, 16f), 0.8f, 0.04f, 4, new Color(0f, 0f, 0.2f), new Color(0.2f, 0.3f, 1f), BodyStyle.Crystalline, new Color(0.3f, 0.4f, 1f)),
                Moon(new Vector2(6f, 19f), 0.7f, 0.04f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.15f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f))
            },
            asteroids: true, asteroidInterval: 5f,
            ortho: 12f, lostDist: 32f,
            star1: 180, star2: 360, star3: 560
        ),
        // 2-11: Two moons, tight gap — crystalline
        MakeLevel(
            new Color(0.08f, 0.02f, 0.12f), new Color(0.6f, 0.3f, 0.9f), BodyStyle.Crystalline, new Color(0.7f, 0.4f, 1f),
            new[] {
                Moon(new Vector2(5f, 12f), 1f, 0.05f, 4, new Color(0.15f, 0.1f, 0f), new Color(1f, 0.7f, 0.1f), BodyStyle.Storm, new Color(1f, 0.8f, 0.2f)),
                Moon(new Vector2(-4f, 14f), 1f, 0.05f, 4, new Color(0f, 0.15f, 0.1f), new Color(0f, 1f, 0.6f), BodyStyle.Oceanic, new Color(0.2f, 1f, 0.7f))
            },
            thrust: 7f, maxSpd: 7f,
            star1: 120, star2: 250, star3: 400
        ),
        // 2-12: Three moons, heavy asteroids + satellites — lava
        MakeLevel(
            new Color(0.2f, 0.02f, 0f), new Color(1f, 0.3f, 0.05f), BodyStyle.LavaWorld, new Color(1f, 0.4f, 0.1f),
            new[] {
                Moon(new Vector2(3f, 13f), 0.9f, 0.05f, 5, new Color(0.05f, 0.1f, 0.15f), new Color(0.3f, 0.7f, 1f), BodyStyle.IceCrystal, new Color(0.4f, 0.8f, 1f),
                    sats: new[] { Sat(2.5f, 0f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.3f, 1f, 0.3f), style: BodyStyle.Toxic) }),
                Moon(new Vector2(-6f, 16f), 0.8f, 0.04f, 4, new Color(0.15f, 0f, 0.15f), new Color(0.9f, 0.2f, 0.9f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 1f)),
                Moon(new Vector2(7f, 19f), 0.7f, 0.04f, 4, new Color(0.1f, 0.1f, 0f), new Color(0.8f, 0.8f, 0.1f), BodyStyle.Mountainous, new Color(0.9f, 0.9f, 0.3f),
                    sats: new[] { Sat(2f, 90f, core: new Color(0.1f, 0f, 0f), rim: new Color(0.8f, 0.1f, 0f), style: BodyStyle.LavaWorld) })
            },
            asteroids: true, asteroidInterval: 4f,
            ortho: 12f, lostDist: 32f,
            star1: 190, star2: 380, star3: 580
        ),
        // 2-13: Two large moons — gas giant
        MakeLevel(
            new Color(0.08f, 0.05f, 0f), new Color(0.8f, 0.6f, 0.1f), BodyStyle.GasGiant, new Color(0.9f, 0.7f, 0.2f),
            new[] {
                Moon(new Vector2(4f, 12f), 1.3f, 0.06f, 6, new Color(0f, 0.1f, 0.2f), new Color(0.1f, 0.6f, 1f), BodyStyle.Storm, new Color(0.2f, 0.7f, 1f),
                    coins: 16, coinR: 3f),
                Moon(new Vector2(-5f, 16f), 1.2f, 0.06f, 6, new Color(0.1f, 0f, 0.1f), new Color(0.7f, 0.2f, 0.7f), BodyStyle.Crystalline, new Color(0.8f, 0.3f, 0.8f),
                    coins: 16, coinR: 3f)
            },
            ortho: 12f, lostDist: 30f,
            star1: 150, star2: 300, star3: 480
        ),
        // 2-14: Three moons, all with satellites + asteroids — mountainous
        MakeLevel(
            new Color(0.12f, 0.08f, 0.02f), new Color(0.8f, 0.6f, 0.2f), BodyStyle.Mountainous, new Color(0.9f, 0.7f, 0.3f),
            new[] {
                Moon(new Vector2(2f, 12f), 0.9f, 0.05f, 5, new Color(0f, 0.15f, 0.15f), new Color(0.1f, 0.9f, 0.9f), BodyStyle.Oceanic, new Color(0.2f, 1f, 1f),
                    sats: new[] { Sat(2.5f, 45f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.2f, 0f), style: BodyStyle.LavaWorld) }),
                Moon(new Vector2(-8f, 15f), 0.8f, 0.05f, 5, new Color(0.1f, 0f, 0.15f), new Color(0.6f, 0.2f, 1f), BodyStyle.NeonMarble, new Color(0.7f, 0.3f, 1f),
                    sats: new[] { Sat(2.5f, 150f, core: new Color(0f, 0.1f, 0f), rim: new Color(0.2f, 0.8f, 0.2f), style: BodyStyle.Toxic) }),
                Moon(new Vector2(7f, 19f), 0.7f, 0.04f, 4, new Color(0.15f, 0.15f, 0f), new Color(1f, 1f, 0.3f), BodyStyle.Crystalline, new Color(1f, 1f, 0.5f),
                    sats: new[] { Sat(2f, 270f, core: new Color(0.1f, 0.05f, 0f), rim: new Color(0.8f, 0.5f, 0.1f), style: BodyStyle.Mountainous) })
            },
            asteroids: true, asteroidInterval: 4f,
            ortho: 13f, lostDist: 34f,
            star1: 200, star2: 400, star3: 620
        ),
        // 2-15: BOSS — oceanic, everything maxed
        MakeLevel(
            new Color(0f, 0.08f, 0.15f), new Color(0f, 0.5f, 1f), BodyStyle.Oceanic, new Color(0.1f, 0.6f, 1f),
            new[] {
                Moon(new Vector2(3f, 12f), 1.1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f),
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(3f, 0f, core: new Color(0.15f, 0.15f, 0f), rim: new Color(1f, 1f, 0.2f), style: BodyStyle.Storm) }),
                Moon(new Vector2(-7f, 16f), 1f, 0.05f, 5, new Color(0f, 0.15f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.Toxic, new Color(0.3f, 1f, 0.4f),
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(2.5f, 120f, core: new Color(0.1f, 0f, 0.15f), rim: new Color(0.6f, 0.2f, 1f), style: BodyStyle.Crystalline) }),
                Moon(new Vector2(6f, 20f), 0.9f, 0.05f, 5, new Color(0.1f, 0.08f, 0f), new Color(0.7f, 0.5f, 0.1f), BodyStyle.Mountainous, new Color(0.8f, 0.6f, 0.2f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 240f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0.1f, 0.8f, 0.8f), style: BodyStyle.Oceanic) })
            },
            asteroids: true, asteroidInterval: 3.5f,
            ortho: 13f, lostDist: 35f,
            star1: 220, star2: 440, star3: 680
        )
    };

    // Chapter 3: Expert — maximum challenge (15 levels)
    static readonly LevelConfig[] Chapter3 = new[]
    {
        // 3-1: Two moons, tiny SOI — storm planet
        MakeLevel(
            new Color(0.08f, 0.05f, 0.12f), new Color(0.5f, 0.3f, 0.9f), BodyStyle.Storm, new Color(0.6f, 0.4f, 1f),
            new[] {
                Moon(new Vector2(3f, 13f), 0.7f, 0.04f, 3, new Color(0.2f, 0.1f, 0f), new Color(1f, 0.6f, 0f), BodyStyle.LavaWorld, new Color(1f, 0.7f, 0.1f)),
                Moon(new Vector2(-4f, 10f), 0.7f, 0.04f, 3, new Color(0f, 0.15f, 0.1f), new Color(0f, 1f, 0.5f), BodyStyle.Toxic, new Color(0.2f, 1f, 0.6f))
            },
            thrust: 6f, maxSpd: 6f,
            star1: 120, star2: 250, star3: 400
        ),
        // 3-2: Three moons, tiny SOI — crystalline
        MakeLevel(
            new Color(0.1f, 0.02f, 0.15f), new Color(0.7f, 0.3f, 1f), BodyStyle.Crystalline, new Color(0.8f, 0.4f, 1f),
            new[] {
                Moon(new Vector2(1f, 12f), 0.7f, 0.04f, 3, new Color(0f, 0.1f, 0.2f), new Color(0.2f, 0.7f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.8f, 1f)),
                Moon(new Vector2(-6f, 14f), 0.6f, 0.04f, 3, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.2f)),
                Moon(new Vector2(5f, 17f), 0.6f, 0.04f, 3, new Color(0f, 0.15f, 0f), new Color(0.3f, 1f, 0.3f), BodyStyle.Oceanic, new Color(0.4f, 1f, 0.4f))
            },
            thrust: 6f, maxSpd: 6f, ortho: 12f, lostDist: 28f,
            star1: 150, star2: 300, star3: 480
        ),
        // 3-3: Two moons + satellites + asteroids — toxic
        MakeLevel(
            new Color(0.06f, 0.12f, 0f), new Color(0.4f, 1f, 0.1f), BodyStyle.Toxic, new Color(0.5f, 1f, 0.2f),
            new[] {
                Moon(new Vector2(4f, 12f), 0.8f, 0.05f, 4, new Color(0.15f, 0f, 0.15f), new Color(0.9f, 0.2f, 0.9f), BodyStyle.NeonMarble, new Color(1f, 0.3f, 1f),
                    sats: new[] { Sat(2f, 0f, core: new Color(0.1f, 0.1f, 0f), rim: new Color(0.8f, 0.8f, 0.1f), style: BodyStyle.Crystalline), Sat(2f, 180f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0f, 0.8f, 0.8f), style: BodyStyle.Storm) }),
                Moon(new Vector2(-5f, 15f), 0.8f, 0.05f, 4, new Color(0.1f, 0.05f, 0f), new Color(0.8f, 0.5f, 0.1f), BodyStyle.Mountainous, new Color(0.9f, 0.6f, 0.2f),
                    sats: new[] { Sat(2f, 90f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.15f, 0f), style: BodyStyle.LavaWorld) })
            },
            asteroids: true, asteroidInterval: 4f,
            star1: 160, star2: 320, star3: 500
        ),
        // 3-4: Four moons! — gas giant
        MakeLevel(
            new Color(0.1f, 0.08f, 0f), new Color(0.9f, 0.7f, 0.1f), BodyStyle.GasGiant, new Color(1f, 0.8f, 0.2f),
            new[] {
                Moon(new Vector2(3f, 11f), 0.7f, 0.04f, 4, new Color(0f, 0.1f, 0.2f), new Color(0.2f, 0.6f, 1f), BodyStyle.IceCrystal, new Color(0.3f, 0.7f, 1f)),
                Moon(new Vector2(-5f, 14f), 0.7f, 0.04f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f)),
                Moon(new Vector2(7f, 17f), 0.6f, 0.04f, 3, new Color(0f, 0.15f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.Toxic, new Color(0.3f, 1f, 0.4f)),
                Moon(new Vector2(-3f, 20f), 0.6f, 0.04f, 3, new Color(0.1f, 0f, 0.15f), new Color(0.6f, 0.2f, 1f), BodyStyle.Crystalline, new Color(0.7f, 0.3f, 1f))
            },
            ortho: 14f, lostDist: 35f,
            star1: 200, star2: 400, star3: 640
        ),
        // 3-5: Single moon, 4 satellites — mountainous
        MakeLevel(
            new Color(0.12f, 0.1f, 0.03f), new Color(0.8f, 0.6f, 0.2f), BodyStyle.Mountainous, new Color(0.9f, 0.7f, 0.3f),
            new[] {
                Moon(new Vector2(0f, 13f), 1.3f, 0.06f, 6, new Color(0.05f, 0.1f, 0.15f), new Color(0.3f, 0.7f, 1f), BodyStyle.Oceanic, new Color(0.4f, 0.8f, 1f),
                    coins: 16, coinR: 3.5f,
                    sats: new[] {
                        Sat(3.5f, 0f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.2f, 0f), style: BodyStyle.LavaWorld),
                        Sat(3.5f, 90f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.3f, 1f, 0.2f), style: BodyStyle.Toxic),
                        Sat(3.5f, 180f, core: new Color(0.1f, 0f, 0.15f), rim: new Color(0.7f, 0.2f, 1f), style: BodyStyle.NeonMarble),
                        Sat(3.5f, 270f, core: new Color(0.15f, 0.15f, 0f), rim: new Color(1f, 1f, 0.2f), style: BodyStyle.Crystalline)
                    })
            },
            star1: 160, star2: 320, star3: 500
        ),
        // 3-6 through 3-15: progressively harder
        MakeLevel(new Color(0.15f, 0f, 0.15f), new Color(1f, 0.1f, 0.8f), BodyStyle.NeonMarble, new Color(1f, 0.2f, 0.9f),
            new[] {
                Moon(new Vector2(5f, 12f), 0.8f, 0.05f, 4, new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.9f, 0.2f), BodyStyle.Storm, new Color(1f, 1f, 0.3f),
                    sats: new[] { Sat(2f, 45f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0f, 0.8f, 0.8f), style: BodyStyle.Oceanic) }),
                Moon(new Vector2(-6f, 15f), 0.8f, 0.05f, 4, new Color(0f, 0f, 0.2f), new Color(0.3f, 0.3f, 1f), BodyStyle.Crystalline, new Color(0.4f, 0.4f, 1f))
            },
            asteroids: true, asteroidInterval: 4f, star1: 150, star2: 300, star3: 480),
        MakeLevel(new Color(0f, 0.12f, 0.1f), new Color(0f, 0.9f, 0.7f), BodyStyle.Oceanic, new Color(0.1f, 1f, 0.8f),
            new[] {
                Moon(new Vector2(2f, 11f), 0.9f, 0.05f, 4, new Color(0.2f, 0.05f, 0f), new Color(1f, 0.4f, 0f), BodyStyle.Mountainous, new Color(1f, 0.5f, 0.1f)),
                Moon(new Vector2(-7f, 14f), 0.8f, 0.04f, 4, new Color(0.1f, 0f, 0.1f), new Color(0.8f, 0.2f, 0.6f), BodyStyle.NeonMarble, new Color(0.9f, 0.3f, 0.7f)),
                Moon(new Vector2(6f, 18f), 0.7f, 0.04f, 3, new Color(0.05f, 0.1f, 0f), new Color(0.4f, 0.9f, 0.1f), BodyStyle.Toxic, new Color(0.5f, 1f, 0.2f))
            },
            asteroids: true, asteroidInterval: 4f, ortho: 12f, lostDist: 30f,
            star1: 170, star2: 340, star3: 540),
        MakeLevel(new Color(0.2f, 0.05f, 0f), new Color(1f, 0.4f, 0.05f), BodyStyle.LavaWorld, new Color(1f, 0.5f, 0.1f),
            new[] {
                Moon(new Vector2(0f, 12f), 1f, 0.05f, 5, new Color(0.05f, 0.05f, 0.15f), new Color(0.4f, 0.4f, 1f), BodyStyle.Storm, new Color(0.5f, 0.5f, 1f),
                    sats: new[] { Sat(2.5f, 0f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.2f, 1f, 0.3f), style: BodyStyle.Toxic), Sat(2.5f, 120f, core: new Color(0.15f, 0.1f, 0f), rim: new Color(0.9f, 0.7f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(-8f, 16f), 0.8f, 0.04f, 4, new Color(0.1f, 0f, 0.15f), new Color(0.6f, 0.2f, 1f), BodyStyle.Crystalline, new Color(0.7f, 0.3f, 1f),
                    sats: new[] { Sat(2f, 180f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.15f, 0f), style: BodyStyle.LavaWorld) })
            },
            asteroids: true, asteroidInterval: 3.5f,
            star1: 160, star2: 320, star3: 520),
        MakeLevel(new Color(0.05f, 0.05f, 0.1f), new Color(0.3f, 0.3f, 0.8f), BodyStyle.IceCrystal, new Color(0.4f, 0.4f, 0.9f),
            new[] {
                Moon(new Vector2(4f, 11f), 0.8f, 0.05f, 4, new Color(0.15f, 0f, 0f), new Color(1f, 0.15f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.25f, 0.1f),
                    sats: new[] { Sat(2f, 30f, core: new Color(0f, 0.1f, 0.15f), rim: new Color(0.2f, 0.7f, 1f), style: BodyStyle.Oceanic) }),
                Moon(new Vector2(-3f, 14f), 0.8f, 0.05f, 4, new Color(0f, 0.15f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.Toxic, new Color(0.3f, 1f, 0.4f)),
                Moon(new Vector2(7f, 18f), 0.7f, 0.04f, 3, new Color(0.1f, 0.05f, 0f), new Color(0.8f, 0.5f, 0.1f), BodyStyle.Mountainous, new Color(0.9f, 0.6f, 0.2f)),
                Moon(new Vector2(-6f, 20f), 0.6f, 0.04f, 3, new Color(0.1f, 0f, 0.1f), new Color(0.7f, 0.2f, 0.7f), BodyStyle.NeonMarble, new Color(0.8f, 0.3f, 0.8f))
            },
            ortho: 14f, lostDist: 35f,
            star1: 200, star2: 420, star3: 660),
        MakeLevel(new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.8f, 0.1f), BodyStyle.Storm, new Color(1f, 0.9f, 0.2f),
            new[] {
                Moon(new Vector2(3f, 12f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.15f), new Color(0.1f, 0.7f, 1f), BodyStyle.Oceanic, new Color(0.2f, 0.8f, 1f),
                    sats: new[] { Sat(2.5f, 60f, core: new Color(0.2f, 0f, 0f), rim: new Color(1f, 0.2f, 0f), style: BodyStyle.LavaWorld), Sat(2.5f, 180f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.3f, 1f, 0.2f), style: BodyStyle.Toxic) }),
                Moon(new Vector2(-7f, 16f), 0.8f, 0.05f, 5, new Color(0.1f, 0f, 0.15f), new Color(0.7f, 0.3f, 1f), BodyStyle.Crystalline, new Color(0.8f, 0.4f, 1f),
                    sats: new[] { Sat(2.5f, 270f, core: new Color(0.15f, 0.1f, 0f), rim: new Color(0.9f, 0.6f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(6f, 20f), 0.7f, 0.04f, 4, new Color(0.15f, 0.15f, 0f), new Color(1f, 1f, 0.3f), BodyStyle.GasGiant, new Color(1f, 1f, 0.5f))
            },
            asteroids: true, asteroidInterval: 3.5f,
            ortho: 13f, lostDist: 34f,
            star1: 210, star2: 430, star3: 670),
        MakeLevel(new Color(0.12f, 0f, 0.08f), new Color(0.9f, 0.1f, 0.5f), BodyStyle.GasGiant, new Color(1f, 0.2f, 0.6f),
            new[] {
                Moon(new Vector2(2f, 11f), 0.8f, 0.05f, 4, new Color(0.1f, 0.1f, 0f), new Color(0.8f, 0.8f, 0.1f), BodyStyle.Mountainous, new Color(0.9f, 0.9f, 0.3f)),
                Moon(new Vector2(-5f, 13f), 0.8f, 0.05f, 4, new Color(0f, 0.1f, 0.15f), new Color(0f, 0.8f, 1f), BodyStyle.Storm, new Color(0.1f, 0.9f, 1f)),
                Moon(new Vector2(7f, 16f), 0.7f, 0.04f, 3, new Color(0.05f, 0.1f, 0f), new Color(0.3f, 0.9f, 0.1f), BodyStyle.Toxic, new Color(0.4f, 1f, 0.2f)),
                Moon(new Vector2(-4f, 19f), 0.7f, 0.04f, 3, new Color(0.1f, 0f, 0.15f), new Color(0.6f, 0.2f, 1f), BodyStyle.NeonMarble, new Color(0.7f, 0.3f, 1f))
            },
            asteroids: true, asteroidInterval: 3f,
            ortho: 14f, lostDist: 35f,
            star1: 220, star2: 450, star3: 700),
        MakeLevel(new Color(0.08f, 0f, 0.12f), new Color(0.5f, 0.2f, 0.9f), BodyStyle.Crystalline, new Color(0.6f, 0.3f, 1f),
            new[] {
                Moon(new Vector2(4f, 11f), 0.9f, 0.05f, 4, new Color(0.2f, 0.05f, 0f), new Color(1f, 0.4f, 0f), BodyStyle.LavaWorld, new Color(1f, 0.5f, 0.1f),
                    sats: new[] { Sat(2f, 0f, core: new Color(0f, 0.1f, 0.1f), rim: new Color(0.1f, 0.8f, 0.8f), style: BodyStyle.Oceanic), Sat(2f, 120f, core: new Color(0.15f, 0.15f, 0f), rim: new Color(1f, 1f, 0.2f), style: BodyStyle.Storm) }),
                Moon(new Vector2(-6f, 14f), 0.9f, 0.05f, 4, new Color(0f, 0.15f, 0f), new Color(0.2f, 1f, 0.3f), BodyStyle.Toxic, new Color(0.3f, 1f, 0.4f),
                    sats: new[] { Sat(2f, 60f, core: new Color(0.1f, 0f, 0f), rim: new Color(0.8f, 0.1f, 0f), style: BodyStyle.LavaWorld), Sat(2f, 240f, core: new Color(0.1f, 0.08f, 0f), rim: new Color(0.7f, 0.5f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(6f, 18f), 0.8f, 0.04f, 4, new Color(0.05f, 0.05f, 0.15f), new Color(0.4f, 0.4f, 1f), BodyStyle.IceCrystal, new Color(0.5f, 0.5f, 1f),
                    sats: new[] { Sat(2f, 180f, core: new Color(0.15f, 0f, 0.15f), rim: new Color(0.9f, 0.2f, 0.9f), style: BodyStyle.NeonMarble) })
            },
            asteroids: true, asteroidInterval: 3f,
            ortho: 13f, lostDist: 33f,
            star1: 230, star2: 460, star3: 720),
        MakeLevel(new Color(0f, 0.08f, 0.12f), new Color(0.1f, 0.6f, 0.9f), BodyStyle.Oceanic, new Color(0.2f, 0.7f, 1f),
            new[] {
                Moon(new Vector2(3f, 11f), 0.8f, 0.05f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.15f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.25f, 0.1f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 0f, core: new Color(0.1f, 0.1f, 0f), rim: new Color(0.9f, 0.9f, 0.2f), style: BodyStyle.Crystalline) }),
                Moon(new Vector2(-6f, 14f), 0.8f, 0.05f, 4, new Color(0.1f, 0f, 0.1f), new Color(0.8f, 0.2f, 0.6f), BodyStyle.NeonMarble, new Color(0.9f, 0.3f, 0.7f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 120f, core: new Color(0f, 0.15f, 0f), rim: new Color(0.3f, 1f, 0.3f), style: BodyStyle.Toxic) }),
                Moon(new Vector2(7f, 18f), 0.7f, 0.04f, 4, new Color(0.05f, 0.05f, 0.1f), new Color(0.4f, 0.3f, 0.9f), BodyStyle.Storm, new Color(0.5f, 0.4f, 1f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 240f, core: new Color(0.12f, 0.08f, 0f), rim: new Color(0.8f, 0.6f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(-3f, 21f), 0.7f, 0.04f, 3, new Color(0.1f, 0.05f, 0f), new Color(0.9f, 0.6f, 0.1f), BodyStyle.GasGiant, new Color(1f, 0.7f, 0.2f),
                    coins: 14, coinR: 2f)
            },
            asteroids: true, asteroidInterval: 2.5f,
            ortho: 15f, lostDist: 38f,
            star1: 250, star2: 500, star3: 780),
        // 3-14: Four moons, satellites + heavy asteroids — toxic
        MakeLevel(new Color(0.06f, 0.12f, 0f), new Color(0.4f, 1f, 0.1f), BodyStyle.Toxic, new Color(0.5f, 1f, 0.2f),
            new[] {
                Moon(new Vector2(4f, 11f), 0.8f, 0.05f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.3f, 0.1f),
                    sats: new[] { Sat(2f, 45f, core: new Color(0f, 0.1f, 0.15f), rim: new Color(0.2f, 0.8f, 1f), style: BodyStyle.IceCrystal) }),
                Moon(new Vector2(-5f, 14f), 0.8f, 0.05f, 4, new Color(0.1f, 0f, 0.15f), new Color(0.7f, 0.2f, 1f), BodyStyle.Crystalline, new Color(0.8f, 0.3f, 1f),
                    sats: new[] { Sat(2f, 150f, core: new Color(0.15f, 0.1f, 0f), rim: new Color(1f, 0.7f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(7f, 17f), 0.7f, 0.04f, 3, new Color(0f, 0.15f, 0.1f), new Color(0.2f, 1f, 0.6f), BodyStyle.Oceanic, new Color(0.3f, 1f, 0.7f)),
                Moon(new Vector2(-3f, 20f), 0.7f, 0.04f, 3, new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.8f, 0.1f), BodyStyle.Storm, new Color(1f, 0.9f, 0.2f))
            },
            asteroids: true, asteroidInterval: 2.5f,
            ortho: 14f, lostDist: 36f,
            star1: 240, star2: 480, star3: 750),
        // 3-15: FINAL BOSS — all styles, max hazards
        MakeLevel(new Color(0.05f, 0.05f, 0.05f), new Color(0.8f, 0.8f, 0.8f), BodyStyle.Mountainous, new Color(0.7f, 0.7f, 0.9f),
            new[] {
                Moon(new Vector2(3f, 11f), 1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.15f, 0.1f), BodyStyle.LavaWorld, new Color(1f, 0.25f, 0.1f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 0f, core: new Color(0f, 0.12f, 0f), rim: new Color(0.3f, 1f, 0.2f), style: BodyStyle.Toxic), Sat(2.5f, 180f, core: new Color(0.1f, 0.1f, 0f), rim: new Color(0.9f, 0.9f, 0.2f), style: BodyStyle.Crystalline) }),
                Moon(new Vector2(-7f, 15f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.2f), new Color(0.1f, 0.7f, 1f), BodyStyle.Storm, new Color(0.2f, 0.8f, 1f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 90f, core: new Color(0.15f, 0f, 0.15f), rim: new Color(0.8f, 0.2f, 0.8f), style: BodyStyle.NeonMarble) }),
                Moon(new Vector2(6f, 19f), 0.8f, 0.05f, 4, new Color(0.05f, 0.1f, 0.1f), new Color(0.2f, 0.8f, 0.7f), BodyStyle.Oceanic, new Color(0.3f, 0.9f, 0.8f),
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 270f, core: new Color(0.12f, 0.08f, 0f), rim: new Color(0.8f, 0.6f, 0.1f), style: BodyStyle.Mountainous) }),
                Moon(new Vector2(-3f, 22f), 0.7f, 0.04f, 4, new Color(0.1f, 0.05f, 0f), new Color(0.9f, 0.6f, 0.1f), BodyStyle.GasGiant, new Color(1f, 0.7f, 0.2f),
                    coins: 14, coinR: 2f)
            },
            asteroids: true, asteroidInterval: 2f,
            ortho: 15f, lostDist: 38f,
            star1: 260, star2: 520, star3: 800)
    };

    static readonly LevelConfig[][] AllChapters = new[] { Chapter1, Chapter2, Chapter3 };

    public static LevelConfig GetLevel(int chapter, int level)
    {
        int ci = chapter - 1;
        int li = level - 1;
        if (ci < 0 || ci >= AllChapters.Length) return null;
        if (li < 0 || li >= AllChapters[ci].Length) return null;
        return AllChapters[ci][li];
    }

    public static LevelConfig GetCurrentLevel()
    {
        return GetLevel(CurrentChapter, CurrentLevel);
    }

    public static int GetChapterCount() => AllChapters.Length;
    public static int GetLevelCount(int chapter)
    {
        int ci = chapter - 1;
        if (ci < 0 || ci >= AllChapters.Length) return 0;
        return AllChapters[ci].Length;
    }

    public static bool AdvanceLevel()
    {
        int ci = CurrentChapter - 1;
        if (ci < AllChapters.Length && CurrentLevel < AllChapters[ci].Length)
        {
            CurrentLevel++;
            return true;
        }
        if (ci + 1 < AllChapters.Length)
        {
            CurrentChapter++;
            CurrentLevel = 1;
            return true;
        }
        return false;
    }

    // --- Level completion tracking (persisted via PlayerPrefs) ---
    static string CompletionKey(int chapter, int level) => $"level_complete_{chapter}_{level}";

    public static bool IsLevelCompleted(int chapter, int level)
    {
        return PlayerPrefs.GetInt(CompletionKey(chapter, level), 0) == 1;
    }

    public static bool IsLevelUnlocked(int chapter, int level)
    {
        if (level == 1 && chapter == 1) return true; // First level always unlocked
        if (level == 1) return IsLevelCompleted(chapter - 1, GetLevelCount(chapter - 1)); // First of new chapter
        return IsLevelCompleted(chapter, level - 1); // Previous level completed
    }

    public static void CompleteLevel(int chapter, int level)
    {
        PlayerPrefs.SetInt(CompletionKey(chapter, level), 1);
        PlayerPrefs.Save();
    }

    // --- High scores ---
    static string HighScoreKey(int chapter, int level) => $"high_score_{chapter}_{level}";
    static string PerfectKey(int chapter, int level) => $"perfect_{chapter}_{level}";

    public static int GetHighScore(int chapter, int level)
    {
        return PlayerPrefs.GetInt(HighScoreKey(chapter, level), 0);
    }

    /// Returns true if this is a new high score
    public static bool SetHighScore(int chapter, int level, int score)
    {
        int prev = GetHighScore(chapter, level);
        if (score > prev)
        {
            PlayerPrefs.SetInt(HighScoreKey(chapter, level), score);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public static bool IsPerfect(int chapter, int level)
    {
        return PlayerPrefs.GetInt(PerfectKey(chapter, level), 0) == 1;
    }

    public static void SetPerfect(int chapter, int level)
    {
        PlayerPrefs.SetInt(PerfectKey(chapter, level), 1);
        PlayerPrefs.Save();
    }
}
