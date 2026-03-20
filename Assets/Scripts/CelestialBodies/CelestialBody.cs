using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float radius = 1f;
    public Color coreColor = new Color(0.1f, 0f, 0.4f);
    public Color rimColor = Color.magenta;
    [Range(64, 512)] public int textureResolution = 256;
    public string surfaceTextureName;
    [Range(0f, 1f)] public float surfaceOpacity = 0.3f;

    protected SpriteRenderer spriteRenderer;
    SpriteRenderer surfaceRenderer;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        GenerateVisual();
        ApplySurfaceTexture();

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

    void ApplySurfaceTexture()
    {
        if (string.IsNullOrEmpty(surfaceTextureName)) return;

        var surfaceTex = Resources.Load<Texture2D>("Sprites/" + surfaceTextureName);
        Debug.Log($"[CelestialBody] Loading surface texture 'Sprites/{surfaceTextureName}': {(surfaceTex != null ? $"{surfaceTex.width}x{surfaceTex.height} readable={surfaceTex.isReadable}" : "NULL")}");
        if (surfaceTex == null)
        {
            Debug.LogWarning($"Surface texture not found: Sprites/{surfaceTextureName}");
            return;
        }

        // Create a circular mask from the body sprite
        var mask = gameObject.AddComponent<SpriteMask>();
        mask.sprite = spriteRenderer.sprite;

        var surfaceObj = new GameObject("Surface");
        surfaceObj.transform.SetParent(transform, false);

        surfaceRenderer = surfaceObj.AddComponent<SpriteRenderer>();
        surfaceRenderer.sortingOrder = 5;
        surfaceRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        // Oversize the surface by 30% so it fills past the vignette edges
        float worldSize = radius * 2f * 1.3f;
        float pixelsPerUnit = surfaceTex.width / worldSize;
        var sprite = Sprite.Create(surfaceTex,
            new Rect(0, 0, surfaceTex.width, surfaceTex.height),
            new Vector2(0.5f, 0.5f),
            pixelsPerUnit);

        surfaceRenderer.sprite = sprite;
        surfaceRenderer.color = new Color(1f, 1f, 1f, surfaceOpacity);
    }
}
