using UnityEngine;
using UnityEngine.InputSystem;

public class RocketController : MonoBehaviour
{
    [Header("Movement")]
    public float thrustForce = 8f;
    public float maxSpeed = 7f;
    public float launchSpeed = 3f;
    public float brakeFactor = 3f;
    public float minSpeed = 0.8f;

    [Header("Visuals")]
    public Color rocketColor = new Color(0f, 1f, 0.4f);
    public Color exhaustColor = new Color(1f, 0.5f, 0f);

    Rigidbody2D rb;
    public bool HasLaunched { get; private set; }
    SpriteRenderer spriteRenderer;
    SpriteRenderer exhaustRenderer;

    void Awake()
    {
        gameObject.tag = "affectedByPlanetGravity";

        // Rigidbody2D setup
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Collider for crash detection
        var col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = 0.2f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 10;

        GenerateRocketSprite();
        CreateExhaust();
    }

    void GenerateRocketSprite()
    {
        int w = 48, h = 72;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color[] clear = new Color[w * h];
        tex.SetPixels(clear);

        int cx = w / 2;
        Color red = new Color(0.85f, 0.1f, 0.1f);
        Color darkRed = new Color(0.6f, 0.05f, 0.05f);
        Color stripe = new Color(0.95f, 0.95f, 0.95f);
        Color window = new Color(0.4f, 0.7f, 1f);
        Color windowHighlight = new Color(0.7f, 0.9f, 1f);
        Color tipColor = new Color(0.9f, 0.9f, 0.9f);
        Color finColor = new Color(0.7f, 0.08f, 0.08f);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float halfWidth = 0f;
                bool filled = false;

                // Nose cone (top 30%)
                if (y >= h * 0.7f)
                {
                    float t = (y - h * 0.7f) / (h * 0.3f);
                    halfWidth = (1f - t) * (w * 0.28f);
                    filled = Mathf.Abs(x - cx) <= halfWidth;
                }
                // Body (15%-70%)
                else if (y >= h * 0.15f)
                {
                    halfWidth = w * 0.28f;
                    filled = Mathf.Abs(x - cx) <= halfWidth;
                }
                // Fins (bottom 15%)
                else
                {
                    float t = (float)y / (h * 0.15f);
                    halfWidth = w * 0.28f + (1f - t) * (w * 0.2f);
                    filled = Mathf.Abs(x - cx) <= halfWidth;
                    if (filled)
                    {
                        // Fin color - darker for the extended fin parts
                        if (Mathf.Abs(x - cx) > w * 0.28f)
                            tex.SetPixel(x, y, finColor);
                        else
                            tex.SetPixel(x, y, red);
                        continue;
                    }
                }

                if (!filled) continue;

                // Window — circular, centered around 60% height
                float wy = h * 0.6f;
                float windowRadius = w * 0.12f;
                float dx = x - cx;
                float dy = y - wy;
                float distToWindow = Mathf.Sqrt(dx * dx + dy * dy);
                if (distToWindow <= windowRadius)
                {
                    float wt = distToWindow / windowRadius;
                    Color wc = Color.Lerp(windowHighlight, window, wt);
                    // Small highlight in upper-left of window
                    if (dx < -windowRadius * 0.2f && dy > windowRadius * 0.2f)
                        wc = Color.Lerp(wc, Color.white, 0.3f);
                    tex.SetPixel(x, y, wc);
                    continue;
                }

                // White racing stripe — vertical, offset to one side
                float stripeCenter = cx + w * 0.12f;
                float stripeWidth = w * 0.06f;
                if (Mathf.Abs(x - stripeCenter) <= stripeWidth && y >= h * 0.15f && y <= h * 0.85f)
                {
                    tex.SetPixel(x, y, stripe);
                    continue;
                }

                // Body shading — lighter on left, darker on right for roundness
                float shade = (float)(x - (cx - halfWidth)) / (halfWidth * 2f);
                Color bodyColor = Color.Lerp(red, darkRed, shade * shade);

                // Edge highlight
                float edgeDist = Mathf.Abs(x - cx) / halfWidth;
                if (edgeDist > 0.85f)
                    bodyColor = Color.Lerp(bodyColor, darkRed, (edgeDist - 0.85f) / 0.15f);

                tex.SetPixel(x, y, bodyColor);
            }
        }

        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.3f), 64f);
        spriteRenderer.sprite = sprite;
    }

    void CreateExhaust()
    {
        var exhaustObj = new GameObject("Exhaust");
        exhaustObj.transform.SetParent(transform, false);
        exhaustObj.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        exhaustRenderer = exhaustObj.AddComponent<SpriteRenderer>();
        exhaustRenderer.sortingOrder = 9;

        int size = 16;
        var tex = new Texture2D(size, size * 2, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        int cx = size / 2;
        int h = size * 2;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float progress = (float)y / h;
                float halfWidth = progress * (size * 0.4f);
                if (Mathf.Abs(x - cx) <= halfWidth)
                {
                    Color c = Color.Lerp(exhaustColor, new Color(1f, 1f, 0f), progress);
                    c.a = progress;
                    tex.SetPixel(x, y, c);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, size, h), new Vector2(0.5f, 1f), 64f);
        exhaustRenderer.sprite = sprite;
        exhaustObj.SetActive(false);
    }

    public bool IsThrusting => CheckThrusting();

    bool CheckThrusting()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) return true;
        if (Mouse.current != null && Mouse.current.leftButton.isPressed) return true;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed) return true;
        return false;
    }

    void Update()
    {
        // Reset scene
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneBootstrap.Reset();
            return;
        }

        bool thrusting = IsThrusting;

        // Show/hide exhaust
        if (exhaustRenderer != null)
            exhaustRenderer.gameObject.SetActive(thrusting && HasLaunched);

        // Launch on first thrust
        if (!HasLaunched && thrusting)
        {
            HasLaunched = true;
            rb.linearVelocity = (Vector2)(transform.up * launchSpeed);
        }

        // Prograde rotation — nose points along velocity
        if (HasLaunched && rb.linearVelocity.magnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void FixedUpdate()
    {
        if (!HasLaunched) return;

        bool thrusting = IsThrusting;

        // Thrust in prograde direction
        if (thrusting)
        {
            Vector2 thrustDir = rb.linearVelocity.magnitude > 0.01f ? rb.linearVelocity.normalized : (Vector2)transform.up;
            rb.AddForce(thrustDir * thrustForce, ForceMode2D.Force);
        }

        // Brake — retrograde drag, can't fully stop
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            if (rb.linearVelocity.magnitude > minSpeed)
            {
                rb.AddForce(-rb.linearVelocity.normalized * brakeFactor, ForceMode2D.Force);
                if (rb.linearVelocity.magnitude < minSpeed)
                    rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
            }
        }

        // Velocity cap
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        if (exhaustRenderer != null)
            exhaustRenderer.gameObject.SetActive(false);

        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null) gm.OnCrashed();
    }
}
