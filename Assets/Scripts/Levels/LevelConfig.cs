using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public PlanetConfig planet;
    public MoonConfig[] moons;
    public RocketConfig rocket;
    public AsteroidConfig asteroids;
    public CameraConfig camera;
    public float lostInSpaceDistance = 25f;
    public int[] starThresholds = { 100, 200, 350 }; // score needed for 1, 2, 3 stars
}

[System.Serializable]
public class PlanetConfig
{
    public float radius = 5f;
    public Color coreColor = new Color(0.1f, 0f, 0.4f);
    public Color rimColor = new Color(1f, 0f, 0.8f);
    public string surfaceTexture = "";
    public float surfaceOpacity = 0.3f;
    public BodyStyle style = BodyStyle.NeonMarble;
    public Color atmosphereColor = new Color(0.5f, 0.8f, 1f);
}

[System.Serializable]
public class MoonConfig
{
    public Vector2 position;
    public float radius = 1f;
    public float mass = 0.05f;
    public int soiRadius = 5;
    public Color coreColor = new Color(0f, 0.2f, 0.4f);
    public Color rimColor = new Color(0f, 1f, 1f);
    public string surfaceTexture = "";
    public float surfaceOpacity = 0.3f;
    public BodyStyle style = BodyStyle.CrateredMoon;
    public Color atmosphereColor = new Color(0.5f, 0.8f, 1f);
    public int coinCount = 12;
    public float coinOrbitRadius = 2.5f;
    public SatelliteConfig[] satellites;
}

[System.Serializable]
public class SatelliteConfig
{
    public float orbitRadius = 2.5f;
    public float startAngle = 0f; // degrees, 0 = right
    public float radius = 0.2f;
    public Color coreColor = new Color(1f, 0.4f, 0f);
    public Color rimColor = new Color(1f, 0.8f, 0.2f);
    public BodyStyle style = BodyStyle.CrateredMoon;
    public Color atmosphereColor = new Color(1f, 0.6f, 0.2f);
}

[System.Serializable]
public class RocketConfig
{
    public float thrustForce = 8f;
    public float maxSpeed = 8f;
    public float launchSpeed = 3f;
    public float brakeFactor = 3f;
}

[System.Serializable]
public class AsteroidConfig
{
    public bool enabled = true;
    public float spawnInterval = 5f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 4f;
    public float minRadius = 0.1f;
    public float maxRadius = 0.25f;
}

[System.Serializable]
public class CameraConfig
{
    public float orthoSize = 10f;
    public Vector2 position = new Vector2(0f, 5f);
}
