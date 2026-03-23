using UnityEngine;

public class Satellite : MonoBehaviour
{
    public float radius = 0.25f;
    public Color coreColor = new Color(1f, 0.4f, 0f);
    public Color rimColor = new Color(1f, 0.8f, 0.2f);
    public BodyStyle bodyStyle = BodyStyle.CrateredMoon;
    public Color atmosphereColor = new Color(1f, 0.6f, 0.2f);

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        gameObject.tag = "affectedByPlanetGravity";
        gameObject.layer = 0;

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = radius;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 4;
    }

    void Start()
    {
        GenerateVisual();
    }

    public void SetOrbitalVelocity(Vector2 velocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = velocity;
    }

    /// Pre-register this satellite as already inside a moon's SOI to skip entry drag
    public void RegisterInSOI(Moon moon)
    {
        // Mark as already tracked so entry drag doesn't fire
        var id = gameObject.GetInstanceID();
        var insideField = typeof(Moon).GetField("insideSOI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (insideField != null)
        {
            var set = insideField.GetValue(moon) as System.Collections.Generic.HashSet<int>;
            set?.Add(id);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ExplosionFX.Spawn(transform.position, rimColor);
        Destroy(gameObject);
    }

    void GenerateVisual()
    {
        int res = 4;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();

        float worldSize = radius * 2f;
        float pixelsPerUnit = res / worldSize;
        var sprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = sprite;
        spriteRenderer.material = BodyPresets.CreateMaterial(bodyStyle, coreColor, rimColor, atmosphereColor);
    }
}
