using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float radius = 1f;
    public Color coreColor = new Color(0.1f, 0f, 0.4f);
    public Color rimColor = Color.magenta;
    public Color atmosphereColor = new Color(0.5f, 0.8f, 1f);
    public BodyStyle bodyStyle = BodyStyle.NeonMarble;

    // Legacy fields kept for compatibility
    public string surfaceTextureName;
    [Range(0f, 1f)] public float surfaceOpacity = 0.3f;

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
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            spriteRenderer.material.SetColor("_ColorA", coreColor);
            spriteRenderer.material.SetColor("_ColorB", rimColor);
        }
    }

    void GenerateVisual()
    {
        // Create a white quad sprite for the shader to render on
        int res = 4;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();

        float worldSize = radius * 2f;
        float pixelsPerUnit = res / worldSize;
        var sprite = Sprite.Create(tex,
            new Rect(0, 0, res, res),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);

        spriteRenderer.sprite = sprite;

        // Apply procedural shader material
        var mat = BodyPresets.CreateMaterial(bodyStyle, coreColor, rimColor, atmosphereColor);
        spriteRenderer.material = mat;
    }
}
