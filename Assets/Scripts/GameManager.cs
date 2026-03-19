using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float lostInSpaceDistance = 25f;

    enum GameState { PreLaunch, Flying, LevelComplete, Crashed, LostInSpace }
    GameState state = GameState.PreLaunch;

    RocketController rocket;
    OrbitDetector orbitDetector;
    Vector3 planetCenter;

    public void Initialize(RocketController rocketRef, Vector3 planetPos)
    {
        rocket = rocketRef;
        planetCenter = planetPos;

        orbitDetector = rocket.gameObject.AddComponent<OrbitDetector>();
        orbitDetector.OnOrbitComplete += OnOrbitComplete;
    }

    void Update()
    {
        if (state == GameState.PreLaunch)
        {
            if (rocket.HasLaunched)
                state = GameState.Flying;
            return;
        }

        if (state == GameState.Flying)
        {
            float dist = Vector2.Distance(rocket.transform.position, planetCenter);
            if (dist > lostInSpaceDistance)
            {
                state = GameState.LostInSpace;
                Debug.Log("LOST IN SPACE");
                FreezeRocket();
            }
        }
    }

    void OnOrbitComplete(Moon moon)
    {
        if (state != GameState.Flying) return;

        Debug.Log($"ORBIT COMPLETE: {moon.gameObject.name}");
        moon.MarkCompleted();

        // Check if all moons are done
        Moon[] allMoons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
        foreach (Moon m in allMoons)
        {
            if (!m.Completed) return;
        }

        state = GameState.LevelComplete;
        Debug.Log("LEVEL COMPLETE!");
    }

    public void OnCrashed()
    {
        if (state != GameState.Flying) return;
        state = GameState.Crashed;
        Debug.Log("CRASHED");
    }

    void FreezeRocket()
    {
        var rb = rocket.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }
}
