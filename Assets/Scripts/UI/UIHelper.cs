using UnityEngine;

public static class UIHelper
{
    public static Sprite GenerateRoundedRect(int w, int h, int radius, bool soft)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = 0, dy = 0;
                if (x < radius) dx = radius - x;
                else if (x > w - radius - 1) dx = x - (w - radius - 1);
                if (y < radius) dy = radius - y;
                else if (y > h - radius - 1) dy = y - (h - radius - 1);

                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist > radius)
                    tex.SetPixel(x, y, Color.clear);
                else if (soft && dist > radius - 4)
                {
                    float a = 1f - (dist - (radius - 4)) / 4f;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * 0.5f));
                }
                else
                    tex.SetPixel(x, y, Color.white);
            }
        }

        tex.Apply();
        var border = new Vector4(radius, radius, radius, radius);
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }

    public static Sprite GenerateRoundedRectBorder(int w, int h, int radius, int thickness)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = 0, dy = 0;
                if (x < radius) dx = radius - x;
                else if (x > w - radius - 1) dx = x - (w - radius - 1);
                if (y < radius) dy = radius - y;
                else if (y > h - radius - 1) dy = y - (h - radius - 1);

                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                bool onEdge = false;
                if (dx > 0 || dy > 0)
                    onEdge = dist <= radius && dist >= radius - thickness;
                else
                    onEdge = x < thickness || x >= w - thickness || y < thickness || y >= h - thickness;

                tex.SetPixel(x, y, onEdge ? Color.white : Color.clear);
            }
        }

        tex.Apply();
        var border = new Vector4(radius, radius, radius, radius);
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
    }
}
