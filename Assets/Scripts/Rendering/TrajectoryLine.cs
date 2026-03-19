using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    public int steps = 80;
    public float timeStep = 0.05f;
    public Color lineColor = new Color(1f, 1f, 1f, 0.15f);

    LineRenderer lr;
    Rigidbody2D rocketRb;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = 0.04f;
        lr.endWidth = 0.02f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0f);
        lr.sortingOrder = 5;
        lr.positionCount = 0;

        var rocket = GameObject.FindWithTag("affectedByPlanetGravity");
        if (rocket != null)
            rocketRb = rocket.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rocketRb == null)
        {
            var rocket = GameObject.FindWithTag("affectedByPlanetGravity");
            if (rocket != null)
                rocketRb = rocket.GetComponent<Rigidbody2D>();
        }

        if (rocketRb == null || !rocketRb.simulated || rocketRb.linearVelocity.magnitude < 0.01f)
        {
            lr.positionCount = 0;
            return;
        }

        Moon[] moons = Object.FindObjectsByType<Moon>(FindObjectsSortMode.None);
        Vector2 pos = rocketRb.position;
        Vector2 vel = rocketRb.linearVelocity;

        lr.positionCount = steps;

        for (int i = 0; i < steps; i++)
        {
            lr.SetPosition(i, new Vector3(pos.x, pos.y, 0f));

            // Simulate gravity from all moons
            foreach (Moon moon in moons)
            {
                Vector2 offset = (Vector2)moon.transform.position - pos;
                float dist = offset.magnitude;

                if (dist < moon.soiRadius && dist > 0.01f)
                {
                    float scaledMass = moon.mass * 100000f;
                    float magsqr = offset.sqrMagnitude;
                    Vector2 gravity = (scaledMass * offset.normalized / magsqr) * rocketRb.mass;
                    vel += gravity * (dist / moon.proximityModifier) * timeStep;
                }
            }

            pos += vel * timeStep;
        }
    }
}
