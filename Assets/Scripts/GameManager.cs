using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float lostInSpaceDistance = 25f;

    enum GameState { PreLaunch, Flying, LevelComplete, Crashed, LostInSpace }
    GameState state = GameState.PreLaunch;

    public bool IsLevelComplete => state == GameState.LevelComplete;

    RocketController rocket;
    OrbitDetector orbitDetector;
    StateOverlay overlay;
    Vector3 planetCenter;

    public void Initialize(RocketController rocketRef, Vector3 planetPos)
    {
        rocket = rocketRef;
        planetCenter = planetPos;

        orbitDetector = rocket.gameObject.AddComponent<OrbitDetector>();
        orbitDetector.OnOrbitComplete += OnOrbitComplete;

        // Create UI overlay
        var overlayObj = new GameObject("StateOverlay");
        overlay = overlayObj.AddComponent<StateOverlay>();
        overlay.Initialize();
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
                FreezeRocket();
                overlay.ShowLostInSpace();
            }
        }
    }

    void OnOrbitComplete(Moon moon)
    {
        if (state != GameState.Flying) return;

        Debug.Log($"ORBIT COMPLETE: {moon.gameObject.name}");
        moon.MarkCompleted();

        if (ScoreDisplay.Instance != null)
            ScoreDisplay.Instance.AddScore(50, moon.transform.position);

        Moon[] allMoons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
        foreach (Moon m in allMoons)
        {
            if (!m.Completed) return;
        }

        // All coins bonus
        int totalCoins = 0;
        foreach (Moon m in allMoons) totalCoins += m.coinCount;
        if (Coin.Collected >= totalCoins && totalCoins > 0 && ScoreDisplay.Instance != null)
            ScoreDisplay.Instance.AddScore(100);

        state = GameState.LevelComplete;
        LevelRegistry.CompleteLevel(LevelRegistry.CurrentChapter, LevelRegistry.CurrentLevel);
        int finalScore = ScoreDisplay.Instance != null ? ScoreDisplay.Instance.score : 0;
        overlay.ShowWin(finalScore);
    }

    public void OnCrashed()
    {
        if (state != GameState.Flying) return;
        state = GameState.Crashed;
        overlay.ShowCrash();
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
