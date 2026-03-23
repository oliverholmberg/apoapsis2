using UnityEngine;

public static class LevelRegistry
{
    public static int CurrentChapter = 1;
    public static int CurrentLevel = 1;

    // Helper to reduce boilerplate
    static LevelConfig MakeLevel(
        Color planetCore, Color planetRim, string planetSurface,
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
                surfaceTexture = planetSurface,
                surfaceOpacity = 0.35f
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
        string tex = "", float texOp = 0.3f, int coins = 12, float coinR = 2.5f, SatelliteConfig[] sats = null)
    {
        return new MoonConfig
        {
            position = pos, radius = r, mass = mass, soiRadius = soi,
            coreColor = core, rimColor = rim,
            surfaceTexture = tex, surfaceOpacity = texOp,
            coinCount = coins, coinOrbitRadius = coinR, satellites = sats
        };
    }

    static SatelliteConfig Sat(float orbitR = 2.5f, float angle = 0f, float r = 0.2f)
    {
        return new SatelliteConfig
        {
            orbitRadius = orbitR, startAngle = angle, radius = r,
            coreColor = new Color(1f, 0.4f, 0f), rimColor = new Color(1f, 0.8f, 0.2f)
        };
    }

    // Chapter 1: Learning the basics (15 levels)
    static readonly LevelConfig[] Chapter1 = new[]
    {
        // 1-1: Single moon, easy orbit
        MakeLevel(
            new Color(0.1f, 0f, 0.4f), new Color(1f, 0f, 0.8f), "surface4",
            new[] { Moon(new Vector2(2f, 14f), 1f, 0.05f, 5, new Color(0f, 0.2f, 0.4f), new Color(0f, 1f, 1f), "surface1", 0.4f) },
            star1: 80, star2: 150, star3: 250
        ),

        // 1-2: Single moon, tighter orbit
        MakeLevel(
            new Color(0.05f, 0.1f, 0.3f), new Color(0.2f, 0.5f, 1f), "surface3",
            new[] { Moon(new Vector2(-1f, 13f), 0.8f, 0.04f, 4, new Color(0.1f, 0.1f, 0.3f), new Color(0.4f, 0.4f, 1f), "surface2", 0.3f) },
            star1: 80, star2: 160, star3: 260
        ),

        // 1-3: Two moons, intro to orbital transfer
        MakeLevel(
            new Color(0.2f, 0f, 0.1f), new Color(1f, 0.3f, 0.4f), "surface4",
            new[] {
                Moon(new Vector2(2f, 14f), 1f, 0.05f, 5, new Color(0f, 0.2f, 0.4f), new Color(0f, 1f, 1f), "surface1", 0.4f),
                Moon(new Vector2(-8f, 11f), 0.8f, 0.05f, 5, new Color(0.3f, 0f, 0.3f), new Color(1f, 0.4f, 1f), "surface2", 0.25f)
            },
            star1: 100, star2: 200, star3: 350
        ),

        // 1-4: Two moons, wider spread
        MakeLevel(
            new Color(0f, 0.15f, 0.1f), new Color(0f, 1f, 0.6f), "surface3",
            new[] {
                Moon(new Vector2(5f, 13f), 1f, 0.05f, 5, new Color(0.2f, 0.15f, 0f), new Color(1f, 0.8f, 0.2f), "surface1", 0.35f),
                Moon(new Vector2(-6f, 15f), 0.9f, 0.05f, 5, new Color(0f, 0.2f, 0.1f), new Color(0.3f, 1f, 0.5f), "surface2", 0.3f)
            },
            star1: 100, star2: 220, star3: 370
        ),

        // 1-5: Single moon + satellite
        MakeLevel(
            new Color(0.15f, 0.05f, 0f), new Color(1f, 0.5f, 0f), "surface4",
            new[] {
                Moon(new Vector2(1f, 13f), 1.1f, 0.05f, 5, new Color(0.15f, 0.1f, 0.3f), new Color(0.6f, 0.4f, 1f), "surface1", 0.35f,
                    sats: new[] { Sat(2.5f, 0f) })
            },
            star1: 100, star2: 200, star3: 320
        ),

        // 1-6: Two moons + satellite on second
        MakeLevel(
            new Color(0.1f, 0f, 0.2f), new Color(0.7f, 0.2f, 1f), "surface3",
            new[] {
                Moon(new Vector2(3f, 14f), 1f, 0.05f, 5, new Color(0.1f, 0.2f, 0f), new Color(0.5f, 1f, 0f), "surface2", 0.3f),
                Moon(new Vector2(-7f, 10f), 0.9f, 0.05f, 5, new Color(0.2f, 0f, 0.1f), new Color(1f, 0.2f, 0.5f), "surface1", 0.35f,
                    sats: new[] { Sat(2.5f, 90f) })
            },
            star1: 110, star2: 230, star3: 380
        ),

        // 1-7: Two moons, asteroids begin
        MakeLevel(
            new Color(0.05f, 0.05f, 0.15f), new Color(0.4f, 0.6f, 1f), "surface4",
            new[] {
                Moon(new Vector2(0f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0.1f, 0f), new Color(1f, 0.6f, 0.1f), "surface1", 0.3f),
                Moon(new Vector2(-9f, 12f), 0.8f, 0.05f, 5, new Color(0f, 0.15f, 0.2f), new Color(0f, 0.8f, 1f), "surface2", 0.3f)
            },
            asteroids: true, asteroidInterval: 8f,
            star1: 120, star2: 250, star3: 400
        ),

        // 1-8: Three moons, no hazards
        MakeLevel(
            new Color(0.2f, 0.05f, 0f), new Color(1f, 0.4f, 0.1f), "surface3",
            new[] {
                Moon(new Vector2(2f, 13f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.3f), new Color(0.2f, 0.6f, 1f), "surface1", 0.3f),
                Moon(new Vector2(-7f, 15f), 0.8f, 0.04f, 4, new Color(0.15f, 0f, 0.2f), new Color(0.8f, 0.3f, 1f), "surface2", 0.3f),
                Moon(new Vector2(8f, 17f), 0.7f, 0.04f, 4, new Color(0.2f, 0.2f, 0f), new Color(1f, 1f, 0.3f), "surface1", 0.25f)
            },
            ortho: 12f, lostDist: 30f,
            star1: 150, star2: 300, star3: 480
        ),

        // 1-9: Two moons, both with satellites
        MakeLevel(
            new Color(0f, 0.1f, 0.15f), new Color(0.1f, 0.8f, 0.8f), "surface4",
            new[] {
                Moon(new Vector2(3f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.3f, 0.2f), "surface2", 0.35f,
                    sats: new[] { Sat(2.5f, 45f) }),
                Moon(new Vector2(-6f, 11f), 0.9f, 0.05f, 5, new Color(0f, 0.2f, 0f), new Color(0.2f, 1f, 0.3f), "surface1", 0.3f,
                    sats: new[] { Sat(2.5f, 180f) })
            },
            star1: 130, star2: 260, star3: 420
        ),

        // 1-10: Single large moon, asteroids, tight space
        MakeLevel(
            new Color(0.15f, 0f, 0.15f), new Color(0.9f, 0.1f, 0.9f), "surface3",
            new[] {
                Moon(new Vector2(0f, 12f), 1.5f, 0.06f, 6, new Color(0.1f, 0.1f, 0.1f), new Color(0.7f, 0.7f, 0.8f), "surface1", 0.4f,
                    coins: 16, coinR: 3f)
            },
            asteroids: true, asteroidInterval: 6f,
            star1: 120, star2: 240, star3: 380
        ),

        // 1-11: Three moons + satellite
        MakeLevel(
            new Color(0.1f, 0.1f, 0f), new Color(0.9f, 0.9f, 0.2f), "surface4",
            new[] {
                Moon(new Vector2(1f, 13f), 0.9f, 0.05f, 5, new Color(0f, 0f, 0.2f), new Color(0.3f, 0.3f, 1f), "surface2", 0.3f),
                Moon(new Vector2(-8f, 16f), 0.8f, 0.04f, 4, new Color(0.2f, 0.1f, 0f), new Color(1f, 0.5f, 0f), "surface1", 0.3f,
                    sats: new[] { Sat(2f, 0f) }),
                Moon(new Vector2(7f, 18f), 0.7f, 0.04f, 4, new Color(0f, 0.2f, 0.15f), new Color(0.2f, 1f, 0.7f), "surface2", 0.25f)
            },
            ortho: 12f, lostDist: 32f,
            star1: 160, star2: 320, star3: 500
        ),

        // 1-12: Two moons, asteroids + satellite
        MakeLevel(
            new Color(0f, 0f, 0.2f), new Color(0.3f, 0.3f, 1f), "surface3",
            new[] {
                Moon(new Vector2(4f, 14f), 1f, 0.05f, 5, new Color(0.2f, 0.05f, 0f), new Color(1f, 0.3f, 0f), "surface1", 0.35f),
                Moon(new Vector2(-5f, 10f), 1f, 0.05f, 5, new Color(0f, 0.15f, 0.1f), new Color(0f, 0.9f, 0.5f), "surface2", 0.3f,
                    sats: new[] { Sat(3f, 120f) })
            },
            asteroids: true, asteroidInterval: 5f,
            star1: 130, star2: 270, star3: 430
        ),

        // 1-13: Three moons, two with satellites
        MakeLevel(
            new Color(0.15f, 0.1f, 0f), new Color(1f, 0.7f, 0.2f), "surface4",
            new[] {
                Moon(new Vector2(2f, 12f), 1f, 0.05f, 5, new Color(0f, 0.1f, 0.2f), new Color(0.1f, 0.6f, 1f), "surface2", 0.3f,
                    sats: new[] { Sat(2.5f, 30f) }),
                Moon(new Vector2(-7f, 15f), 0.8f, 0.05f, 5, new Color(0.15f, 0f, 0.1f), new Color(0.9f, 0.2f, 0.5f), "surface1", 0.3f,
                    sats: new[] { Sat(2.5f, 210f) }),
                Moon(new Vector2(6f, 19f), 0.7f, 0.04f, 4, new Color(0.1f, 0.15f, 0f), new Color(0.6f, 1f, 0f), "surface2", 0.25f)
            },
            ortho: 12f, lostDist: 32f,
            star1: 170, star2: 340, star3: 540
        ),

        // 1-14: Three moons + asteroids
        MakeLevel(
            new Color(0.1f, 0f, 0.1f), new Color(0.8f, 0.2f, 0.6f), "surface3",
            new[] {
                Moon(new Vector2(0f, 13f), 1f, 0.05f, 5, new Color(0.15f, 0.15f, 0f), new Color(1f, 1f, 0.4f), "surface1", 0.35f),
                Moon(new Vector2(-9f, 11f), 0.9f, 0.05f, 5, new Color(0f, 0.1f, 0.15f), new Color(0.2f, 0.7f, 0.9f), "surface2", 0.3f,
                    sats: new[] { Sat(2.5f, 60f) }),
                Moon(new Vector2(7f, 17f), 0.8f, 0.04f, 4, new Color(0.2f, 0f, 0f), new Color(1f, 0.2f, 0.1f), "surface1", 0.3f)
            },
            asteroids: true, asteroidInterval: 5f,
            ortho: 12f, lostDist: 32f,
            star1: 180, star2: 360, star3: 560
        ),

        // 1-15: BOSS — Three moons, all with satellites, asteroids
        MakeLevel(
            new Color(0.05f, 0.05f, 0.05f), new Color(0.9f, 0.9f, 0.9f), "surface4",
            new[] {
                Moon(new Vector2(3f, 13f), 1.1f, 0.05f, 5, new Color(0.2f, 0f, 0f), new Color(1f, 0.15f, 0.1f), "surface1", 0.35f,
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(3f, 0f) }),
                Moon(new Vector2(-8f, 16f), 0.9f, 0.05f, 5, new Color(0f, 0f, 0.2f), new Color(0.2f, 0.3f, 1f), "surface2", 0.3f,
                    coins: 14, coinR: 2.8f, sats: new[] { Sat(2.5f, 90f) }),
                Moon(new Vector2(6f, 20f), 0.8f, 0.05f, 5, new Color(0f, 0.2f, 0f), new Color(0.1f, 1f, 0.2f), "surface1", 0.3f,
                    coins: 14, coinR: 2.5f, sats: new[] { Sat(2.5f, 180f) })
            },
            asteroids: true, asteroidInterval: 4f,
            ortho: 13f, lostDist: 35f,
            star1: 200, star2: 400, star3: 620
        )
    };

    static readonly LevelConfig[][] AllChapters = new[] { Chapter1 };

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
}
