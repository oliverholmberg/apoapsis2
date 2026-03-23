using UnityEngine;
using UnityEngine.UI;

public class MenuCarousel : MonoBehaviour
{
    public static MenuCarousel Instance { get; private set; }

    int currentStop; // 0=title, 1=ch1, 2=ch2, 3=ch3
    GameObject clockPivot;
    float currentAngle;
    float targetAngle;
    bool rotating;
    float rotateDuration = 0.6f; // seconds for full transition
    float rotateTimer;
    float startAngle;

    Font titleFont;
    Font bodyFont;

    // Clock layout
    float clockRadius = 18f;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        titleFont = Resources.Load<Font>("Fonts/Orbitron-Bold");
        bodyFont = Resources.Load<Font>("Fonts/Orbitron-Regular");

        // Camera fixed, looking at top of clock
        var cam = Camera.main;
        cam.orthographicSize = 10f;
        cam.transform.position = new Vector3(0f, clockRadius, -10f);
        cam.transform.rotation = Quaternion.identity;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f);

        // Clock pivot — everything rotates around this
        clockPivot = new GameObject("ClockPivot");
        clockPivot.transform.position = Vector3.zero;

        // Starfield on the clock
        var starsObj = new GameObject("MenuStarfield");
        starsObj.transform.SetParent(clockPivot.transform, false);
        var sf = starsObj.AddComponent<Starfield>();
        sf.starCount = 1200;
        sf.rotationSpeed = 0.5f;

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Build screens at clock positions
        // 12 o'clock (top):    position (0, R),   rotation 0°
        // 3 o'clock (right):   position (R, 0),   rotation -90° (content rotated CW)
        // 6 o'clock (bottom):  position (0, -R),  rotation 180° (upside down)
        // 9 o'clock (left):    position (-R, 0),  rotation 90° (content rotated CCW)
        BuildScreen(0, new Vector3(0f, clockRadius, 0f), 0f, BuildTitleContent);
        BuildScreen(1, new Vector3(clockRadius, 0f, 0f), -90f, (parent) => BuildChapterContent(parent, 1));
        // Only build additional chapters if they exist
        if (LevelRegistry.GetChapterCount() >= 2)
            BuildScreen(2, new Vector3(0f, -clockRadius, 0f), 180f, (parent) => BuildChapterContent(parent, 2));
        if (LevelRegistry.GetChapterCount() >= 3)
            BuildScreen(3, new Vector3(-clockRadius, 0f, 0f), 90f, (parent) => BuildChapterContent(parent, 3));

        currentStop = 0;
        currentAngle = 0f;
        targetAngle = 0f;
    }

    void BuildScreen(int index, Vector3 position, float zRotation, System.Action<Transform> buildContent)
    {
        // World-space canvas at the clock position
        var screenObj = new GameObject($"Screen_{index}");
        screenObj.transform.SetParent(clockPivot.transform, false);
        screenObj.transform.localPosition = position;
        screenObj.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        var canvas = screenObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 50;

        var scaler = screenObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        screenObj.AddComponent<GraphicRaycaster>();

        // Size the canvas to fill the camera view (~20 x 20 world units)
        var rect = screenObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1920, 1080);
        rect.localScale = new Vector3(0.018f, 0.018f, 1f); // 1920 * 0.018 = ~34 world units, oversized for readability

        buildContent(screenObj.transform);
    }

    void BuildTitleContent(Transform parent)
    {
        // Title text via world-space Text
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        var titleText = titleObj.AddComponent<Text>();
        titleText.font = titleFont;
        titleText.text = "APOAPSIS";
        titleText.fontSize = 140;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(0.6f, 0.8f, 1f);
        titleText.raycastTarget = false;
        var titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(800, 100);
        titleRect.anchoredPosition = new Vector2(0f, 200f);

        // Subtitle
        var subObj = new GameObject("Subtitle");
        subObj.transform.SetParent(parent, false);
        var subText = subObj.AddComponent<Text>();
        subText.font = bodyFont;
        subText.text = "AN ORBITAL PUZZLE GAME";
        subText.fontSize = 42;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.5f, 0.4f, 0.8f, 0.7f);
        subText.raycastTarget = false;
        var subRect = subObj.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.5f, 0.5f);
        subRect.anchorMax = new Vector2(0.5f, 0.5f);
        subRect.sizeDelta = new Vector2(800, 40);
        subRect.anchoredPosition = new Vector2(0f, 110f);

        // Play button
        CreateButton(parent, "PLAY", new Vector2(0f, -30f), new Vector2(300, 70),
            new Color(0.1f, 0.5f, 0.3f, 0.9f), 56, () => NavigateTo(1));

        // Credits button
        CreateButton(parent, "CREDITS", new Vector2(0f, -130f), new Vector2(260, 80),
            new Color(0.2f, 0.2f, 0.4f, 0.9f), 42, () => { });
    }

    void BuildChapterContent(Transform parent, int chapter)
    {
        // Header
        var headerObj = new GameObject("Header");
        headerObj.transform.SetParent(parent, false);
        var headerText = headerObj.AddComponent<Text>();
        headerText.font = titleFont;
        headerText.text = "SELECT LEVEL";
        headerText.fontSize = 100;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleCenter;
        headerText.color = new Color(0.6f, 0.8f, 1f);
        headerText.raycastTarget = false;
        var headerRect = headerObj.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.5f, 0.5f);
        headerRect.anchorMax = new Vector2(0.5f, 0.5f);
        headerRect.sizeDelta = new Vector2(600, 60);
        headerRect.anchoredPosition = new Vector2(0f, 380f);

        // Chapter label
        var chObj = new GameObject("ChapterLabel");
        chObj.transform.SetParent(parent, false);
        var chText = chObj.AddComponent<Text>();
        chText.font = bodyFont;
        chText.text = $"CHAPTER {ToRoman(chapter)}";
        chText.fontSize = 42;
        chText.alignment = TextAnchor.MiddleCenter;
        chText.color = new Color(0.5f, 0.4f, 0.8f, 0.7f);
        chText.raycastTarget = false;
        var chRect = chObj.GetComponent<RectTransform>();
        chRect.anchorMin = new Vector2(0.5f, 0.5f);
        chRect.anchorMax = new Vector2(0.5f, 0.5f);
        chRect.sizeDelta = new Vector2(600, 35);
        chRect.anchoredPosition = new Vector2(0f, 320f);

        // Level grid
        int count = LevelRegistry.GetLevelCount(chapter);
        int cols = 5;
        float spacing = 120f;
        float startX = -(cols - 1) * spacing / 2f;
        float startY = 180f;

        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            CreateLevelButton(parent, chapter, i + 1,
                new Vector2(startX + col * spacing, startY - row * spacing));
        }

        // Back button
        CreateButton(parent, "BACK", new Vector2(0f, -380f), new Vector2(220, 80),
            new Color(0.3f, 0.15f, 0.15f, 0.9f), 40, () => NavigateTo(0));
    }

    void CreateLevelButton(Transform parent, int chapter, int level, Vector2 pos)
    {
        var config = LevelRegistry.GetLevel(chapter, level);
        if (config == null) return;

        var pc = config.planet;

        var btnObj = new GameObject($"Level_{chapter}_{level}");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(90, 90);
        btnRect.anchoredPosition = pos;

        // Render planet shader to texture for this button
        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = RenderPlanetSprite(pc.style, pc.coreColor, pc.rimColor, pc.atmosphereColor);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.3f, 1.3f, 1.3f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = colors;
        btn.onClick.AddListener(() =>
        {
            Hide();
            SceneBootstrap.LoadLevel(chapter, level);
        });

        // Atmosphere ring using the planet's atmosphere color
        var ringObj = new GameObject("Ring");
        ringObj.transform.SetParent(btnObj.transform, false);
        var ringRect = ringObj.AddComponent<RectTransform>();
        ringRect.anchorMin = Vector2.zero;
        ringRect.anchorMax = Vector2.one;
        ringRect.sizeDelta = new Vector2(14, 14);
        var ringImg = ringObj.AddComponent<Image>();
        ringImg.sprite = GenerateCircleRing(pc.atmosphereColor);
        ringImg.color = new Color(1f, 1f, 1f, 0.5f);
        ringImg.raycastTarget = false;

        // Label
        var lblObj = new GameObject("Label");
        lblObj.transform.SetParent(btnObj.transform, false);
        var lbl = lblObj.AddComponent<Text>();
        lbl.font = titleFont;
        lbl.text = $"{level}";
        lbl.fontSize = 50;
        lbl.fontStyle = FontStyle.Bold;
        lbl.alignment = TextAnchor.MiddleCenter;
        lbl.color = Color.white;
        lbl.raycastTarget = false;
        var lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;
    }

    // --- NAVIGATION ---
    // Rotate CW to bring next screen up: each stop is -90° further
    public void NavigateTo(int stop)
    {
        if (rotating || stop == currentStop) return;
        float from = currentStop * 90f;
        float to = stop * 90f;
        targetAngle = currentAngle + Mathf.DeltaAngle(currentAngle, to);
        startAngle = currentAngle;
        rotateTimer = 0f;
        currentStop = stop;
        rotating = true;
    }

    // Smooth ease-in-out curve
    static float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    void Update()
    {
        if (!rotating) return;

        rotateTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(rotateTimer / rotateDuration);
        float eased = EaseInOut(t);

        currentAngle = Mathf.Lerp(startAngle, targetAngle, eased);

        if (t >= 1f)
        {
            currentAngle = targetAngle;
            rotating = false;
        }

        clockPivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        clockPivot.SetActive(true);

        var cam = Camera.main;
        cam.transform.position = new Vector3(0f, clockRadius, -10f);
        cam.transform.rotation = Quaternion.identity;
        cam.orthographicSize = 10f;

        clockPivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        clockPivot.SetActive(false);
    }

    public void ReturnToLevelSelect()
    {
        Show();
        int stop = LevelRegistry.CurrentChapter;
        // Snap to the chapter's angle immediately, then show
        currentAngle = stop * 90f;
        targetAngle = currentAngle;
        currentStop = stop;
        clockPivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    // --- HELPERS ---
    void CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, Color color, int fontSize, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject(label + "Btn");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = pos;

        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = UIHelper.GenerateRoundedRect(64, 32, 14, false);
        btnImg.type = Image.Type.Sliced;
        btnImg.color = color;

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(Mathf.Min(color.r + 0.15f, 1f), Mathf.Min(color.g + 0.15f, 1f), Mathf.Min(color.b + 0.15f, 1f), 1f);
        colors.pressedColor = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f, 1f);
        btn.colors = colors;
        btn.onClick.AddListener(action);

        var bdrObj = new GameObject("Border");
        bdrObj.transform.SetParent(btnObj.transform, false);
        var bdrRect = bdrObj.AddComponent<RectTransform>();
        bdrRect.anchorMin = Vector2.zero;
        bdrRect.anchorMax = Vector2.one;
        bdrRect.sizeDelta = Vector2.zero;
        var bdrImg = bdrObj.AddComponent<Image>();
        bdrImg.sprite = UIHelper.GenerateRoundedRectBorder(64, 32, 14, 2);
        bdrImg.type = Image.Type.Sliced;
        bdrImg.color = new Color(colors.highlightedColor.r, colors.highlightedColor.g, colors.highlightedColor.b, 0.4f);
        bdrImg.raycastTarget = false;

        var lblObj = new GameObject("Label");
        lblObj.transform.SetParent(btnObj.transform, false);
        var lbl = lblObj.AddComponent<Text>();
        lbl.font = bodyFont;
        lbl.text = label;
        lbl.fontSize = fontSize;
        lbl.fontStyle = FontStyle.Bold;
        lbl.alignment = TextAnchor.MiddleCenter;
        lbl.color = Color.white;
        lbl.raycastTarget = false;
        lbl.verticalOverflow = VerticalWrapMode.Overflow;
        lbl.horizontalOverflow = HorizontalWrapMode.Overflow;
        var lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;
    }

    Sprite RenderPlanetSprite(BodyStyle style, Color coreColor, Color rimColor, Color atmoColor)
    {
        int res = 128;
        var mat = BodyPresets.CreateMaterial(style, coreColor, rimColor, atmoColor);

        // Render the shader to a RenderTexture
        var rt = RenderTexture.GetTemporary(res, res, 0, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Bilinear;

        // For off-screen rendering, disable blending so alpha writes correctly
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);

        // Clear to transparent before rendering
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = prev;

        Graphics.Blit(null, rt, mat);

        // Read back to Texture2D
        RenderTexture.active = rt;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.ReadPixels(new Rect(0, 0, res, res), 0, 0);
        tex.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        Object.Destroy(mat);

        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    Sprite GenerateCircleSprite(Color rimColor)
    {
        int res = 64;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        int half = res / 2;
        Color core = new Color(rimColor.r * 0.3f, rimColor.g * 0.3f, rimColor.b * 0.3f, 1f);
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float d = Mathf.Sqrt((x - half) * (x - half) + (y - half) * (y - half)) / half;
                tex.SetPixel(x, y, d > 1f ? Color.clear : Color.Lerp(core, rimColor, d * d));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    Sprite GenerateCircleRing(Color color)
    {
        int res = 64;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        int half = res / 2;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float d = Mathf.Sqrt((x - half) * (x - half) + (y - half) * (y - half));
                float inner = half - 4, outer = half - 1;
                if (d >= inner && d <= outer)
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, Mathf.Clamp01(Mathf.Min(d - inner, outer - d) / 2f)));
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    static string ToRoman(int n)
    {
        return n switch
        {
            1 => "I", 2 => "II", 3 => "III", 4 => "IV",
            5 => "V", 6 => "VI", 7 => "VII", 8 => "VIII",
            9 => "IX", 10 => "X",
            _ => n.ToString()
        };
    }
}
