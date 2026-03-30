using UnityEngine;
using UnityEngine.InputSystem;

public class RocketController : MonoBehaviour
{
    [Header("Movement")]
    public float thrustForce = 8f;
    public float maxSpeed = 8f;
    public float launchSpeed = 3f;
    public float brakeFactor = 3f;
    public float minSpeed = 0.8f;

    [Header("Visuals")]
    public Color rocketColor = new Color(0f, 1f, 0.4f);
    public Color exhaustColor = new Color(1f, 0.5f, 0f);

    Rigidbody2D rb;
    public bool HasLaunched { get; private set; }
    bool dead;
    SpriteRenderer spriteRenderer;
    SpriteRenderer exhaustRenderer;
    ParticleSystem exhaustParticles;
    RocketTrail trail;

    // Near miss tracking
    public float nearMissRadius = 0.8f;
    readonly System.Collections.Generic.HashSet<int> nearMissed = new();

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

        // Near-miss trigger zone
        var nearMissTrigger = gameObject.AddComponent<CircleCollider2D>();
        nearMissTrigger.radius = nearMissRadius;
        nearMissTrigger.isTrigger = true;

        GenerateRocketSprite();
        CreateExhaust();
        CreateTrail();
    }

    void GenerateRocketSprite()
    {
        var tex = Resources.Load<Texture2D>("Sprites/rocket2");
        if (tex != null)
        {
            // Pivot at 0.5, 0.3 — bottom-heavy so exhaust aligns
            float pixelsPerUnit = tex.height / 1.2f; // ~1.2 world units tall
            var sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.3f),
                pixelsPerUnit);
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("Rocket sprite not found at Resources/Sprites/rocket");
        }
    }

    void CreateExhaust()
    {
        // Static flame sprite
        var exhaustObj = new GameObject("Exhaust");
        exhaustObj.transform.SetParent(transform, false);
        exhaustObj.transform.localPosition = new Vector3(0f, -0.4f, 0f);

        exhaustRenderer = exhaustObj.AddComponent<SpriteRenderer>();
        exhaustRenderer.sortingOrder = 9;

        int w = 24, h = 48;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] clear = new Color[w * h];
        tex.SetPixels(clear);

        int cx = w / 2;
        for (int y = 0; y < h; y++)
        {
            float t = (float)y / h; // 0 at bottom (tip), 1 at top (base)
            float halfWidth = t * (w * 0.45f);
            for (int x = 0; x < w; x++)
            {
                float dx = Mathf.Abs(x - cx);
                if (dx <= halfWidth)
                {
                    float edgeFade = 1f - (dx / Mathf.Max(halfWidth, 0.01f));
                    // Hot white core → orange → red tip
                    Color c;
                    if (t > 0.7f)
                        c = Color.Lerp(new Color(1f, 0.9f, 0.7f), new Color(1f, 1f, 0.9f), edgeFade * 0.5f);
                    else if (t > 0.3f)
                        c = Color.Lerp(new Color(1f, 0.4f, 0f), new Color(1f, 0.7f, 0.2f), edgeFade);
                    else
                        c = Color.Lerp(new Color(0.8f, 0.1f, 0f), exhaustColor, edgeFade);

                    c.a = Mathf.Lerp(0.3f, 0.9f, t) * edgeFade;
                    tex.SetPixel(x, y, c);
                }
            }
        }
        tex.Apply();
        var sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 1f), 48f);
        exhaustRenderer.sprite = sprite;
        exhaustObj.SetActive(false);

        // Particle emitter for sparks
        var particleObj = new GameObject("ExhaustParticles");
        particleObj.transform.SetParent(transform, false);
        particleObj.transform.localPosition = new Vector3(0f, -0.5f, 0f);
        particleObj.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

        exhaustParticles = particleObj.AddComponent<ParticleSystem>();
        var main = exhaustParticles.main;
        main.startLifetime = 0.3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.08f);
        main.maxParticles = 60;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.6f, 0.1f),
            new Color(1f, 0.9f, 0.4f)
        );

        var emission = exhaustParticles.emission;
        emission.rateOverTime = 80;

        var shape = exhaustParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        shape.radius = 0.05f;

        var sizeOverLifetime = exhaustParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

        var colorOverLifetime = exhaustParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var alphaGrad = new Gradient();
        alphaGrad.SetKeys(
            new GradientColorKey[] { new(new Color(1f, 0.7f, 0.3f), 0f), new(new Color(1f, 0.2f, 0f), 1f) },
            new GradientAlphaKey[] { new(0.8f, 0f), new(0f, 1f) }
        );
        colorOverLifetime.color = alphaGrad;

        var renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 8;

        exhaustParticles.Stop();
    }

    void CreateTrail()
    {
        trail = gameObject.AddComponent<RocketTrail>();
    }

    public bool IsThrusting => InputManager.Instance != null && InputManager.Instance.IsThrusting();

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
        bool showExhaust = thrusting && HasLaunched && !dead;
        if (exhaustRenderer != null)
            exhaustRenderer.gameObject.SetActive(showExhaust);
        if (exhaustParticles != null)
        {
            if (showExhaust && !exhaustParticles.isPlaying) exhaustParticles.Play();
            else if (!showExhaust && exhaustParticles.isPlaying) exhaustParticles.Stop();
        }

        // Trail emits whenever launched
        if (trail != null)
            trail.SetEmitting(HasLaunched);

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

        CheckMoonNearMiss();
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
            if (HapticManager.Instance != null) HapticManager.Instance.ThrustTick();
        }
        else
        {
            if (HapticManager.Instance != null) HapticManager.Instance.ThrustStop();
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
        dead = true;
        HapticManager.Instance?.Crash();
        ExplosionFX.Spawn(transform.position, new Color(1f, 0.4f, 0.1f));

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        if (exhaustRenderer != null)
            exhaustRenderer.gameObject.SetActive(false);
        if (exhaustParticles != null)
            exhaustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (trail != null)
            trail.SetEmitting(false);
        spriteRenderer.enabled = false;

        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null) gm.OnCrashed();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!HasLaunched) return;

        // No near misses after level complete
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null && gm.IsLevelComplete) return;

        // Near miss with satellites or asteroids (score once per object)
        bool isHazard = other.GetComponent<Satellite>() != null || other.GetComponent<Asteroid>() != null;
        if (!isHazard) return;

        int id = other.gameObject.GetInstanceID();
        if (nearMissed.Contains(id)) return;
        nearMissed.Add(id);

        if (ScoreDisplay.Instance != null)
            ScoreDisplay.Instance.AddScore(25, other.transform.position);
        HapticManager.Instance?.NearMiss();
    }

    void CheckMoonNearMiss()
    {
        if (!HasLaunched) return;

        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null && gm.IsLevelComplete) return;

        Moon[] moons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
        foreach (Moon m in moons)
        {
            int id = m.gameObject.GetInstanceID();
            if (nearMissed.Contains(id)) continue;

            float dist = Vector2.Distance(transform.position, m.transform.position);
            float surfaceDist = dist - m.radius;
            if (surfaceDist > 0f && surfaceDist < nearMissRadius)
            {
                nearMissed.Add(id);
                if (ScoreDisplay.Instance != null)
                    ScoreDisplay.Instance.AddScore(25, m.transform.position);
                HapticManager.Instance?.NearMiss();
            }
        }
    }
}
