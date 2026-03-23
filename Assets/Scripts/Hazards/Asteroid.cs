using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float radius = 0.15f;

    static Texture2D[] sprites;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    static void LoadSprites()
    {
        if (sprites != null) return;
        sprites = new Texture2D[3];
        for (int i = 0; i < 3; i++)
            sprites[i] = Resources.Load<Texture2D>($"Sprites/asteroid{i + 1}");
    }

    void Awake()
    {
        gameObject.tag = "affectedByPlanetGravity";

        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 0.1f;

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = radius;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 4;

        LoadSprites();
        AssignRandomSprite();

        // Random rotation
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }

    void AssignRandomSprite()
    {
        var tex = sprites[Random.Range(0, sprites.Length)];
        if (tex == null) return;

        float worldSize = radius * 2f;
        float pixelsPerUnit = tex.width / worldSize;
        var sprite = Sprite.Create(tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);
        spriteRenderer.sprite = sprite;
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Explode on any collision
        ExplosionFX.Spawn(transform.position, new Color(0.8f, 0.5f, 0.2f));
        Destroy(gameObject);
    }
}
