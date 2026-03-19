using UnityEngine;

public class StarSparkle : MonoBehaviour
{
    public Color baseColor;

    SpriteRenderer sr;
    float speed;
    float offset;
    float minAlpha;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        speed = Random.Range(1f, 4f);
        offset = Random.Range(0f, Mathf.PI * 2f);
        minAlpha = Random.Range(0.1f, 0.4f);
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed + offset) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, 1f, t);
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
    }
}
