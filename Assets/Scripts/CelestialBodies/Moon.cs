using UnityEngine;

public class Moon : CelestialBody
{
    [Header("Gravity")]
    public float mass = 1f;
    public int soiRadius = 4;
    public int proximityModifier = 195;

    [Header("SOI Visual")]
    public Color soiColor = new Color(0f, 1f, 1f, 0.3f);
    public float soiLineWidth = 0.08f;

    [Header("SOI Entry")]
    public float soiEntrySpeedReduction = 0.35f; // multiplier applied once on entry

    float scaledMass;
    public bool Completed { get; private set; }
    readonly System.Collections.Generic.HashSet<int> insideSOI = new();

    public void MarkCompleted()
    {
        Completed = true;
        Recolor(new Color(0.8f, 0.8f, 0.8f), Color.white);
    }

    [Header("Coins")]
    public int coinCount = 12;
    public float coinOrbitRadius = 2.5f;

    protected override void Start()
    {
        base.Start();
        spriteRenderer.sortingOrder = 2;
        scaledMass = mass * 100000f;
        CreateSOIVisual();
        SpawnCoins();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            float angle = (float)i / coinCount * Mathf.PI * 2f;
            var coinObj = new GameObject("Coin");
            coinObj.transform.SetParent(transform, false);
            coinObj.transform.localPosition = new Vector3(
                Mathf.Cos(angle) * coinOrbitRadius,
                Mathf.Sin(angle) * coinOrbitRadius,
                0f
            );
            coinObj.AddComponent<Coin>();
        }
    }

    void FixedUpdate()
    {
        GameObject[] affected = GameObject.FindGameObjectsWithTag("affectedByPlanetGravity");

        foreach (GameObject gravBody in affected)
        {
            Rigidbody2D rb = gravBody.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            float orbitalDistance = Vector3.Distance(transform.position, rb.transform.position);

            if (orbitalDistance < soiRadius)
            {
                // Slow down on SOI entry — more braking when heading straight at the moon
                int id = gravBody.GetInstanceID();
                if (!insideSOI.Contains(id))
                {
                    insideSOI.Add(id);
                    rb.linearDamping = 0.05f; // restore damping inside SOI
                    Vector2 toMoon = ((Vector2)transform.position - rb.position).normalized;
                    Vector2 velDir = rb.linearVelocity.normalized;
                    float headOn = Mathf.Clamp01(Vector2.Dot(velDir, toMoon)); // 1 = straight at moon, 0 = tangential
                    float reduction = Mathf.Lerp(1f, soiEntrySpeedReduction, headOn);
                    rb.linearVelocity *= reduction;
                }

                Vector3 offset = transform.position - rb.transform.position;
                offset.z = 0;

                float magsqr = offset.sqrMagnitude;

                if (magsqr > 0.0001f)
                {
                    Vector3 gravityVector = (scaledMass * offset.normalized / magsqr) * rb.mass;
                    float gravScale = 1f;

                    // Reduce gravity while thrusting to make orbit escapes easier
                    var rocket = gravBody.GetComponent<RocketController>();
                    if (rocket != null && rocket.IsThrusting)
                        gravScale = 0.4f;

                    rb.AddForce(gravityVector * (orbitalDistance / proximityModifier) * gravScale);
                }
            }
            else
            {
                if (insideSOI.Remove(gravBody.GetInstanceID()))
                {
                    // Disable damping when leaving all SOIs
                    bool inAnySoi = false;
                    Moon[] allMoons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
                    foreach (Moon m in allMoons)
                    {
                        if (m.insideSOI.Contains(gravBody.GetInstanceID())) { inAnySoi = true; break; }
                    }
                    if (!inAnySoi)
                        rb.linearDamping = 0f;
                }
            }
        }
    }

    void CreateSOIVisual()
    {
        var soiObj = new GameObject("SOI");
        soiObj.transform.SetParent(transform, false);

        var circle = soiObj.AddComponent<CircleRenderer>();
        circle.radius = soiRadius;
        circle.color = soiColor;
        circle.lineWidth = soiLineWidth;
        circle.segments = 96;
        circle.sortingOrder = 1;

        // Outer glow layer — wider, more transparent
        var glowObj = new GameObject("SOI_Glow");
        glowObj.transform.SetParent(transform, false);

        var glow = glowObj.AddComponent<CircleRenderer>();
        glow.radius = soiRadius;
        glow.color = new Color(soiColor.r, soiColor.g, soiColor.b, soiColor.a * 0.3f);
        glow.lineWidth = soiLineWidth * 4f;
        glow.segments = 96;
    }
}
