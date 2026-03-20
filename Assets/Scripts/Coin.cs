using UnityEngine;

public class Coin : MonoBehaviour
{
    public float magnetRadius = 0.8f;
    public float magnetStrength = 12f;
    public float collectRadius = 0.3f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.05f;

    SpriteRenderer sr;
    Vector3 basePos;
    Transform originalParent;
    float bobOffset;
    static int collected;
    public static int Collected => collected;

    public static void ResetCount() => collected = 0;

    void Start()
    {
        basePos = transform.localPosition;
        originalParent = transform.parent;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        // Generate coin sprite
        int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color gold = new Color(1f, 0.85f, 0.2f);
        Color bright = new Color(1f, 0.95f, 0.5f);
        int half = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float r = half - 0.5f;

                if (dist <= r)
                {
                    float t = dist / r;
                    Color c = Color.Lerp(bright, gold, t);
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 96f);

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 5;
    }

    void Update()
    {
        // Find rocket specifically
        var rocketComp = Object.FindFirstObjectByType<RocketController>();
        var rocket = rocketComp != null ? rocketComp.gameObject : null;
        if (rocket == null)
        {
            transform.localPosition = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmount;
            return;
        }

        float dist = Vector2.Distance(transform.position, rocket.transform.position);

        // Collect
        if (dist < collectRadius)
        {
            collected++;
            if (ScoreDisplay.Instance != null)
                ScoreDisplay.Instance.AddScore(10, transform.position);
            Destroy(gameObject);
            return;
        }

        // Magnet — unparent so world-space movement works
        if (dist < magnetRadius)
        {
            if (transform.parent != null)
                transform.SetParent(null, true);

            Vector2 dir = ((Vector2)rocket.transform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * magnetStrength * Time.deltaTime);
        }
        else
        {
            // Reparent back to moon if we drifted away
            if (transform.parent == null && originalParent != null)
            {
                transform.SetParent(originalParent, true);
                basePos = transform.localPosition;
            }
            transform.localPosition = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmount;
        }
    }
}
