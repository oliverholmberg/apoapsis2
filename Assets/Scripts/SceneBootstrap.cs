using UnityEngine;

public class SceneBootstrap : MonoBehaviour
{
    static SceneBootstrap instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        var go = new GameObject("SceneBootstrap");
        instance = go.AddComponent<SceneBootstrap>();
        DontDestroyOnLoad(go);
        instance.BuildScene();
    }

    public static void Reset()
    {
        instance.ClearScene();
        instance.BuildScene();
    }

    void ClearScene()
    {
        var world = GameObject.Find("World");
        if (world != null) Destroy(world);

        var rocket = GameObject.Find("Rocket");
        if (rocket != null) Destroy(rocket);

        var stars = GameObject.Find("Starfield");
        if (stars != null) Destroy(stars);

        var traj = GameObject.Find("TrajectoryLine");
        if (traj != null) Destroy(traj);

        Coin.ResetCount();

        var gm = GameObject.Find("GameManager");
        if (gm != null) Destroy(gm);

        // Reset camera rotation from world rotation
        var cam = Camera.main;
        cam.transform.rotation = Quaternion.identity;
    }

    void BuildScene()
    {
        // Camera setup
        var cam = Camera.main;
        cam.orthographicSize = 10f;
        cam.transform.position = new Vector3(0f, 5f, -10f);
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f);

        // Starfield
        var stars = new GameObject("Starfield");
        stars.AddComponent<Starfield>();

        // World pivot — centered on planet, rotates planet + moon together
        var world = new GameObject("World");
        world.transform.position = new Vector3(0f, -7f, 0f);
        world.AddComponent<WorldRotation>();

        // Planet
        var planet = new GameObject("Planet");
        planet.transform.SetParent(world.transform, false);
        var planetBody = planet.AddComponent<CelestialBody>();
        planetBody.radius = 5f;
        planetBody.coreColor = new Color(0.1f, 0f, 0.4f);
        planetBody.rimColor = new Color(1f, 0f, 0.8f);

        // Planet rim glow
        var rim = new GameObject("Rim");
        rim.transform.SetParent(planet.transform, false);
        var rimCircle = rim.AddComponent<CircleRenderer>();
        rimCircle.radius = 5f;
        rimCircle.color = new Color(1f, 0f, 0.8f, 0.8f);
        rimCircle.lineWidth = 0.1f;
        rimCircle.sortingOrder = 1;

        // Moon
        var moonObj = new GameObject("Moon");
        moonObj.transform.SetParent(world.transform, false);
        moonObj.transform.localPosition = new Vector3(2f, 14f, 0f);
        var moon = moonObj.AddComponent<Moon>();
        moon.radius = 1f;
        moon.coreColor = new Color(0f, 0.2f, 0.4f);
        moon.rimColor = new Color(0f, 1f, 1f);
        moon.soiRadius = 5;
        moon.mass = 0.05f;

        // Moon 2
        var moon2Obj = new GameObject("Moon2");
        moon2Obj.transform.SetParent(world.transform, false);
        moon2Obj.transform.localPosition = new Vector3(-8f, 11f, 0f);
        var moon2 = moon2Obj.AddComponent<Moon>();
        moon2.radius = 0.8f;
        moon2.coreColor = new Color(0.3f, 0f, 0.3f);
        moon2.rimColor = new Color(1f, 0.4f, 1f);
        moon2.soiRadius = 4;
        moon2.mass = 0.04f;

        // Rocket
        var rocketObj = new GameObject("Rocket");
        rocketObj.transform.position = new Vector3(0f, -1.7f, 0f);
        var rocket = rocketObj.AddComponent<RocketController>();

        // Trajectory prediction
        var trajObj = new GameObject("TrajectoryLine");
        trajObj.AddComponent<TrajectoryLine>();

        // Game Manager
        var gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<GameManager>();
        gm.Initialize(rocket, world.transform.position);
    }
}
