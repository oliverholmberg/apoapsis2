using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    public float fieldSize = 50f;

    LineRenderer lr;
    Vector3 position;
    Vector3 direction;
    float speed;
    float lifetime;
    float timer;
    float trailLength;
    float maxAlpha;

    void Start()
    {
        // Start near the visible area (camera sees ~20 units)
        float visibleRange = 12f;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        position = new Vector3(
            Random.Range(-visibleRange, visibleRange),
            Random.Range(-visibleRange, visibleRange),
            0f
        );

        // Random diagonal direction
        float dirAngle = Random.Range(0f, Mathf.PI * 2f);
        direction = new Vector3(Mathf.Cos(dirAngle), Mathf.Sin(dirAngle), 0f);

        speed = Random.Range(20f, 40f);
        trailLength = Random.Range(2f, 4f);
        lifetime = Random.Range(0.3f, 0.6f);
        maxAlpha = Random.Range(0.4f, 0.7f);

        lr = gameObject.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.positionCount = 2;
        lr.startWidth = 0.03f;
        lr.endWidth = 0f;
        lr.sortingOrder = -9;
        lr.useWorldSpace = false;

        // Slight color variation
        float tint = Random.Range(0.85f, 1f);
        lr.startColor = new Color(1f, tint, tint * 0.9f, maxAlpha);
        lr.endColor = new Color(1f, tint, tint * 0.9f, 0f);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        timer += dt;
        position += direction * speed * dt;

        // Fade in then out
        float t = timer / lifetime;
        float alpha;
        if (t < 0.2f)
            alpha = maxAlpha * (t / 0.2f);
        else
            alpha = maxAlpha * (1f - (t - 0.2f) / 0.8f);
        alpha = Mathf.Max(alpha, 0f);

        Color c = lr.startColor;
        lr.startColor = new Color(c.r, c.g, c.b, alpha);
        lr.endColor = new Color(c.r, c.g, c.b, 0f);

        Vector3 tail = position - direction * trailLength;
        lr.SetPosition(0, position);
        lr.SetPosition(1, tail);

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
