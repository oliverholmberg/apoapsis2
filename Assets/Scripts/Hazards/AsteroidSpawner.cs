using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public float spawnInterval = 5f;
    public float minSpeed = 1.5f;
    public float maxSpeed = 4f;
    public float spawnDistance = 18f;
    public float minRadius = 0.1f;
    public float maxRadius = 0.25f;

    float timer;
    Vector3 levelCenter;

    public void Initialize(Vector3 center)
    {
        levelCenter = center;
        timer = spawnInterval; // spawn first one after interval
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // Only spawn after rocket has launched
        var rc = Object.FindFirstObjectByType<RocketController>();
        if (rc == null || !rc.HasLaunched) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnAsteroid();
            timer = spawnInterval + Random.Range(-spawnInterval * 0.3f, spawnInterval * 0.3f);
        }
    }

    void SpawnAsteroid()
    {
        // Spawn from random position along the top/sides arc
        float angle = Random.Range(0f, Mathf.PI); // top half circle
        Vector3 spawnPos = levelCenter + new Vector3(
            Mathf.Cos(angle) * spawnDistance,
            Mathf.Sin(angle) * spawnDistance,
            0f
        );

        // Aim roughly toward the level center with some spread
        Vector2 toCenter = ((Vector2)levelCenter - (Vector2)spawnPos).normalized;
        float spread = Random.Range(-30f, 30f) * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(
            toCenter.x * Mathf.Cos(spread) - toCenter.y * Mathf.Sin(spread),
            toCenter.x * Mathf.Sin(spread) + toCenter.y * Mathf.Cos(spread)
        );

        float speed = Random.Range(minSpeed, maxSpeed);

        var obj = new GameObject("Asteroid");
        obj.transform.position = spawnPos;
        var asteroid = obj.AddComponent<Asteroid>();
        asteroid.radius = Random.Range(minRadius, maxRadius);
        asteroid.SetVelocity(dir * speed);
    }
}
