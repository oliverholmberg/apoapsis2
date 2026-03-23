using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class RocketTrail : MonoBehaviour
{
    TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.time = 0.6f;
        trail.startWidth = 0.08f;
        trail.endWidth = 0f;
        trail.minVertexDistance = 0.1f;
        trail.numCapVertices = 3;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.sortingOrder = 8;

        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[]
            {
                new(new Color(0.3f, 0.6f, 1f), 0f),
                new(new Color(0.1f, 0.2f, 0.5f), 1f)
            },
            new GradientAlphaKey[]
            {
                new(0.25f, 0f),
                new(0f, 1f)
            }
        );
        trail.colorGradient = grad;
        trail.emitting = false;
    }

    public void SetEmitting(bool emit)
    {
        if (trail != null) trail.emitting = emit;
    }

    public void ClearTrail()
    {
        if (trail != null) trail.Clear();
    }
}
