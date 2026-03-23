using UnityEngine;

public class SceneBootstrap : MonoBehaviour
{
    static SceneBootstrap instance;
    static MenuCarousel carousel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        var go = new GameObject("SceneBootstrap");
        instance = go.AddComponent<SceneBootstrap>();
        DontDestroyOnLoad(go);

        // Create persistent menu carousel
        var carouselObj = new GameObject("MenuCarousel");
        carousel = carouselObj.AddComponent<MenuCarousel>();
        carousel.Initialize();
    }

    public static void Reset()
    {
        instance.ClearScene();
        instance.BuildScene();
    }

    public static void LoadLevel(int chapter, int level)
    {
        LevelRegistry.CurrentChapter = chapter;
        LevelRegistry.CurrentLevel = level;
        instance.ClearScene();
        instance.BuildScene();
    }

    public static void ShowMenu()
    {
        instance.ClearScene();
        if (carousel != null)
            carousel.ReturnToLevelSelect();
    }

    void ClearScene()
    {
        var world = GameObject.Find("World");
        if (world != null) Destroy(world);

        var rocket = GameObject.Find("Rocket");
        if (rocket != null) Destroy(rocket);

        foreach (var obj in GameObject.FindGameObjectsWithTag("affectedByPlanetGravity"))
            if (obj != null && (obj.GetComponent<Satellite>() != null || obj.GetComponent<Asteroid>() != null))
                Destroy(obj);

        var spawner = GameObject.Find("AsteroidSpawner");
        if (spawner != null) Destroy(spawner);

        var scoreDisp = GameObject.Find("ScoreDisplay");
        if (scoreDisp != null) Destroy(scoreDisp);

        var stars = GameObject.Find("Starfield");
        if (stars != null) Destroy(stars);

        var traj = GameObject.Find("TrajectoryLine");
        if (traj != null) Destroy(traj);

        Coin.ResetCount();

        var gm = GameObject.Find("GameManager");
        if (gm != null) Destroy(gm);

        var overlay = GameObject.Find("StateOverlay");
        if (overlay != null) Destroy(overlay);

        var input = GameObject.Find("InputManager");
        if (input != null) Destroy(input);

        var pause = GameObject.Find("PauseMenu");
        if (pause != null) { Time.timeScale = 1f; Destroy(pause); }

        // Reset camera rotation from world rotation
        var cam = Camera.main;
        cam.transform.rotation = Quaternion.identity;
    }

    void BuildScene()
    {
        // Hide menu carousel during gameplay
        if (carousel != null)
            carousel.Hide();

        var config = LevelRegistry.GetCurrentLevel();
        if (config == null)
        {
            Debug.LogError($"No level config for {LevelRegistry.CurrentChapter}-{LevelRegistry.CurrentLevel}");
            return;
        }

        BuildFromConfig(config);
    }

    void BuildFromConfig(LevelConfig config)
    {
        // Camera setup
        var cam = Camera.main;
        cam.orthographicSize = config.camera.orthoSize;
        cam.transform.position = new Vector3(config.camera.position.x, config.camera.position.y, -10f);
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f);

        // Input Manager
        var inputObj = new GameObject("InputManager");
        inputObj.AddComponent<InputManager>();

        // Starfield
        var stars = new GameObject("Starfield");
        stars.AddComponent<Starfield>();

        // World pivot — centered on planet
        var world = new GameObject("World");
        world.transform.position = new Vector3(0f, -config.planet.radius - 2f, 0f);
        world.AddComponent<WorldRotation>();

        // Planet
        var planet = new GameObject("Planet");
        planet.transform.SetParent(world.transform, false);
        var planetBody = planet.AddComponent<CelestialBody>();
        planetBody.radius = config.planet.radius;
        planetBody.coreColor = config.planet.coreColor;
        planetBody.rimColor = config.planet.rimColor;
        planetBody.bodyStyle = config.planet.style;
        planetBody.atmosphereColor = config.planet.atmosphereColor;

        // Planet rim glow
        var rim = new GameObject("Rim");
        rim.transform.SetParent(planet.transform, false);
        var rimCircle = rim.AddComponent<CircleRenderer>();
        rimCircle.radius = config.planet.radius;
        rimCircle.color = new Color(config.planet.atmosphereColor.r, config.planet.atmosphereColor.g, config.planet.atmosphereColor.b, 0.8f);
        rimCircle.lineWidth = 0.1f;
        rimCircle.sortingOrder = 1;

        // Moons
        for (int i = 0; i < config.moons.Length; i++)
        {
            var mc = config.moons[i];
            var moonObj = new GameObject($"Moon{(i > 0 ? (i + 1).ToString() : "")}");
            moonObj.transform.SetParent(world.transform, false);
            moonObj.transform.localPosition = new Vector3(mc.position.x, mc.position.y, 0f);
            var moon = moonObj.AddComponent<Moon>();
            moon.radius = mc.radius;
            moon.mass = mc.mass;
            moon.soiRadius = mc.soiRadius;
            moon.coreColor = mc.coreColor;
            moon.rimColor = mc.rimColor;
            moon.bodyStyle = mc.style;
            moon.atmosphereColor = mc.atmosphereColor;
            moon.coinCount = mc.coinCount;
            moon.coinOrbitRadius = mc.coinOrbitRadius;

            // Satellites
            if (mc.satellites != null)
            {
                foreach (var sc in mc.satellites)
                {
                    float angle = sc.startAngle * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * sc.orbitRadius, Mathf.Sin(angle) * sc.orbitRadius, 0f);

                    var satObj = new GameObject("Satellite");
                    satObj.transform.position = moonObj.transform.TransformPoint(offset);
                    var sat = satObj.AddComponent<Satellite>();
                    sat.radius = sc.radius;
                    sat.coreColor = sc.coreColor;
                    sat.rimColor = sc.rimColor;

                    // Orbital velocity tangent to position
                    float scaledMass = mc.mass * 100000f;
                    float orbitalSpeed = Mathf.Sqrt(scaledMass / moon.proximityModifier);
                    Vector2 tangent = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
                    sat.SetOrbitalVelocity(tangent * orbitalSpeed);
                    sat.RegisterInSOI(moon);
                }
            }
        }

        // Rocket
        float rocketY = world.transform.position.y + config.planet.radius + 0.3f;
        var rocketObj = new GameObject("Rocket");
        rocketObj.transform.position = new Vector3(0f, rocketY, 0f);
        var rocket = rocketObj.AddComponent<RocketController>();
        rocket.thrustForce = config.rocket.thrustForce;
        rocket.maxSpeed = config.rocket.maxSpeed;
        rocket.launchSpeed = config.rocket.launchSpeed;
        rocket.brakeFactor = config.rocket.brakeFactor;

        // Trajectory prediction
        var trajObj = new GameObject("TrajectoryLine");
        trajObj.AddComponent<TrajectoryLine>();

        // Game Manager
        var gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<GameManager>();
        gm.lostInSpaceDistance = config.lostInSpaceDistance;
        gm.Initialize(rocket, world.transform.position);

        // Score Display
        var scoreObj = new GameObject("ScoreDisplay");
        scoreObj.AddComponent<ScoreDisplay>();

        // Asteroid Spawner
        if (config.asteroids.enabled)
        {
            var spawnerObj = new GameObject("AsteroidSpawner");
            var spawner = spawnerObj.AddComponent<AsteroidSpawner>();
            spawner.spawnInterval = config.asteroids.spawnInterval;
            spawner.minSpeed = config.asteroids.minSpeed;
            spawner.maxSpeed = config.asteroids.maxSpeed;
            spawner.minRadius = config.asteroids.minRadius;
            spawner.maxRadius = config.asteroids.maxRadius;
            spawner.Initialize(world.transform.position);
        }

        // Pause Menu
        var pauseObj = new GameObject("PauseMenu");
        var pause = pauseObj.AddComponent<PauseMenu>();
        pause.Initialize();
    }
}
