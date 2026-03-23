using UnityEngine;

public class ExplosionFX : MonoBehaviour
{
    public static void Spawn(Vector3 position, Color color)
    {
        var obj = new GameObject("Explosion");
        obj.transform.position = position;
        var fx = obj.AddComponent<ExplosionFX>();
        fx.CreateParticles(color);
    }

    void CreateParticles(Color color)
    {
        var ps = gameObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = 0.8f;
        main.startSpeed = 4f;
        main.startSize = 0.15f;
        main.maxParticles = 40;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new ParticleSystem.MinMaxGradient(
            color,
            new Color(1f, 0.8f, 0.2f)
        );
        main.gravityModifier = 0.5f;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 30) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var alphaGrad = new Gradient();
        alphaGrad.SetKeys(
            new GradientColorKey[] { new(Color.white, 0f), new(Color.white, 1f) },
            new GradientAlphaKey[] { new(1f, 0f), new(0f, 1f) }
        );
        colorOverLifetime.color = alphaGrad;

        // Renderer
        var renderer = GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 15;

        ps.Play();
        Destroy(gameObject, 1.5f);
    }
}
