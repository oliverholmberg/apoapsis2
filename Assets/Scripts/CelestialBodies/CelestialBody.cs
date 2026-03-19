using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float radius = 1f;
    public Color coreColor = new Color(0.1f, 0f, 0.4f);
    public Color rimColor = Color.magenta;
    [Range(64, 512)] public int textureResolution = 256;

    protected SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        GenerateVisual();

        // Collision surface
        var col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = radius;
    }

    public void Recolor(Color newCore, Color newRim)
    {
        coreColor = newCore;
        rimColor = newRim;
        GenerateVisual();
    }

    void GenerateVisual()
    {
        var tex = new Texture2D(textureResolution, textureResolution, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        int half = textureResolution / 2;
        float maxDist = half;

        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                float dx = x - half;
                float dy = y - half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float t = dist / maxDist;

                if (t > 1f)
                {
                    tex.SetPixel(x, y, Color.clear);
                }
                else
                {
                    Color c = Color.Lerp(coreColor, rimColor, t * t);
                    c.a = 1f;
                    tex.SetPixel(x, y, c);
                }
            }
        }

        tex.Apply();

        float worldSize = radius * 2f;
        float pixelsPerUnit = textureResolution / worldSize;
        var sprite = Sprite.Create(tex,
            new Rect(0, 0, textureResolution, textureResolution),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);

        spriteRenderer.sprite = sprite;
    }
}
