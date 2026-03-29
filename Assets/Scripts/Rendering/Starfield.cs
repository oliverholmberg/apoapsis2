using UnityEngine;

public class Starfield : MonoBehaviour
{
    public int starCount = 1000;
    public float fieldSize = 50f;
    public float minSize = 0.02f;
    public float maxSize = 0.08f;
    public float sparkleChance = 0.15f;
    public float rotationSpeed = 2f;

    // Background effects
    public int nebulaCount = 3;
    public int auroraCount = 2;

    // Shooting stars
    float shootingStarTimer;
    float nextShootingStar = 1f; // first one comes quickly

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * dt);

        shootingStarTimer += dt;
        if (shootingStarTimer >= nextShootingStar)
        {
            shootingStarTimer = 0f;
            nextShootingStar = Random.Range(3f, 8f);
            SpawnShootingStar();
        }
    }

    void SpawnShootingStar()
    {
        var obj = new GameObject("ShootingStar");
        obj.transform.SetParent(transform, false);
        var ss = obj.AddComponent<ShootingStar>();
        ss.fieldSize = fieldSize;
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

        SpawnNebulae();
        SpawnAuroras();
    }

    void SpawnNebulae()
    {
        var shader = Shader.Find("Custom/Nebula");
        if (shader == null) return;

        // Color palettes for variety
        Color[][] palettes = new Color[][]
        {
            new Color[] { new Color(0.4f, 0.1f, 0.6f), new Color(0.1f, 0.3f, 0.8f), new Color(1.0f, 0.4f, 0.7f) },
            new Color[] { new Color(0.1f, 0.2f, 0.5f), new Color(0.0f, 0.5f, 0.7f), new Color(0.3f, 0.8f, 1.0f) },
            new Color[] { new Color(0.5f, 0.05f, 0.2f), new Color(0.8f, 0.2f, 0.1f), new Color(1.0f, 0.7f, 0.3f) },
        };

        for (int i = 0; i < nebulaCount; i++)
        {
            var obj = new GameObject($"Nebula_{i}");
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = new Vector3(
                Random.Range(-fieldSize * 0.6f, fieldSize * 0.6f),
                Random.Range(-fieldSize * 0.6f, fieldSize * 0.6f),
                0.1f
            );

            float size = Random.Range(8f, 18f);
            obj.transform.localScale = new Vector3(size, size, 1f);
            obj.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -11;

            // Create quad sprite
            var quadTex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    quadTex.SetPixel(x, y, Color.white);
            quadTex.Apply();
            sr.sprite = Sprite.Create(quadTex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);

            var mat = new Material(shader);
            var palette = palettes[i % palettes.Length];
            mat.SetColor("_ColorA", palette[0]);
            mat.SetColor("_ColorB", palette[1]);
            mat.SetColor("_ColorC", palette[2]);
            mat.SetFloat("_Alpha", Random.Range(0.08f, 0.18f));
            mat.SetFloat("_SpiralStrength", Random.Range(1.5f, 3.5f));
            mat.SetFloat("_WarpStrength", Random.Range(0.8f, 1.8f));
            mat.SetFloat("_NoiseScale", Random.Range(2.5f, 4.5f));
            mat.SetFloat("_DriftSpeed", Random.Range(0.005f, 0.015f));
            mat.SetFloat("_CoreBrightness", Random.Range(1.0f, 2.0f));
            sr.material = mat;
        }
    }

    void SpawnAuroras()
    {
        var shader = Shader.Find("Custom/Aurora");
        if (shader == null) return;

        Color[][] palettes = new Color[][]
        {
            new Color[] { new Color(0.1f, 0.8f, 0.4f), new Color(0.2f, 0.5f, 0.9f), new Color(0.6f, 0.2f, 0.8f) },
            new Color[] { new Color(0.0f, 0.6f, 0.8f), new Color(0.3f, 0.2f, 0.9f), new Color(0.8f, 0.1f, 0.5f) },
        };

        for (int i = 0; i < auroraCount; i++)
        {
            var obj = new GameObject($"Aurora_{i}");
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = new Vector3(
                Random.Range(-fieldSize * 0.4f, fieldSize * 0.4f),
                Random.Range(-fieldSize * 0.3f, fieldSize * 0.3f),
                0.1f
            );

            // Auroras are wide and tall
            float width = Random.Range(15f, 25f);
            float height = Random.Range(8f, 14f);
            obj.transform.localScale = new Vector3(width, height, 1f);
            obj.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-20f, 20f));

            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = -11;

            var quadTex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    quadTex.SetPixel(x, y, Color.white);
            quadTex.Apply();
            sr.sprite = Sprite.Create(quadTex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);

            var mat = new Material(shader);
            var palette = palettes[i % palettes.Length];
            mat.SetColor("_ColorA", palette[0]);
            mat.SetColor("_ColorB", palette[1]);
            mat.SetColor("_ColorC", palette[2]);
            mat.SetFloat("_Alpha", Random.Range(0.08f, 0.15f));
            mat.SetFloat("_FlowSpeed", Random.Range(0.15f, 0.4f));
            mat.SetFloat("_WaveFreq", Random.Range(2.5f, 4.5f));
            mat.SetFloat("_WaveAmp", Random.Range(0.1f, 0.2f));
            mat.SetFloat("_Thickness", Random.Range(0.06f, 0.12f));
            mat.SetFloat("_Layers", Random.Range(2f, 4f));
            sr.material = mat;
        }
    }
}
