using UnityEngine;

public class Coin : MonoBehaviour
{
    public float magnetRadius = 4f;
    public float magnetStrength = 10f;
    public float collectRadius = 0.3f;
    public float bobSpeed = 2f;
    public float bobAmount = 0.05f;

    SpriteRenderer sr;
    Vector3 basePos;
    float bobOffset;
    static int collected;
    public static int Collected => collected;

    public static void ResetCount() => collected = 0;

    void Start()
    {
        basePos = transform.localPosition;
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
        var sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 48f);

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 5;
    }

    void Update()
    {
        // Gentle bob
        transform.localPosition = basePos + Vector3.up * Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmount;

        // Find rocket
        var rocket = GameObject.FindWithTag("affectedByPlanetGravity");
        if (rocket == null) return;

        float dist = Vector2.Distance(transform.position, rocket.transform.position);

        // Collect
        if (dist < collectRadius)
        {
            collected++;
            Destroy(gameObject);
            return;
        }

        // Magnet
        if (dist < magnetRadius)
        {
            Vector2 dir = ((Vector2)rocket.transform.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(dir * magnetStrength * Time.deltaTime);
        }
    }
}
