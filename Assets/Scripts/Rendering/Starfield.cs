using UnityEngine;

public class Starfield : MonoBehaviour
{
    public int starCount = 1000;
    public float fieldSize = 50f;
    public float minSize = 0.02f;
    public float maxSize = 0.08f;
    public float sparkleChance = 0.15f;
    public float rotationSpeed = 2f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    void Start()
    {
        var tex = new Texture2D(4, 4);
        tex.filterMode = FilterMode.Point;
        for (int y = 0; y < 4; y++)
            for (int x = 0; x < 4; x++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();

        var sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);

        for (int i = 0; i < starCount; i++)
        {
            var star = new GameObject("Star");
            star.transform.SetParent(transform, false);
            star.transform.localPosition = new Vector3(
                Random.Range(-fieldSize, fieldSize),
                Random.Range(-fieldSize, fieldSize),
                0f
            );

            float size = Random.Range(minSize, maxSize);
            star.transform.localScale = new Vector3(size, size, 1f);

            var sr = star.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = -10;

            float brightness = Random.Range(0.3f, 1f);
            float tint = Random.Range(0f, 1f);
            Color c;
            if (tint < 0.7f)
                c = new Color(brightness, brightness, brightness);
            else if (tint < 0.85f)
                c = new Color(brightness, brightness * 0.8f, brightness * 0.6f);
            else
                c = new Color(brightness * 0.7f, brightness * 0.8f, brightness);
            sr.color = c;

            // Some stars sparkle
            if (Random.value < sparkleChance)
            {
                var sparkle = star.AddComponent<StarSparkle>();
                sparkle.baseColor = c;
            }
        }
    }
}
