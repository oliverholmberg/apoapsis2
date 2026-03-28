using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuCarousel : MonoBehaviour
{
    public static MenuCarousel Instance { get; private set; }

    int currentStop; // 0=title, 1=ch1, 2=ch2, 3=ch3
    public GameObject ClockPivot => clockPivot;
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
    GameObject[] screenObjects;

    // Title fly-in animation
    RectTransform titleRect;
    RectTransform subtitleRect;
    CanvasGroup titleGroup;
    CanvasGroup subtitleGroup;
    float titleAnimTimer;
    float titleAnimDuration = 2.5f;
    bool titleAnimating;

    // Swipe navigation
    bool swipeTracking;
    Vector2 swipeStartPos;
    float swipeMinDistance = 80f; // minimum pixels to count as swipe

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    float menuOrthoSize = 10f;

    static float GetPhoneZoom()
    {
        float aspect = (float)Screen.width / Screen.height;
        if (aspect > 1f) aspect = 1f / aspect;
        return aspect < 0.65f ? 0.82f : 1f;
    }

    public void Initialize()
    {
        titleFont = Resources.Load<Font>("Fonts/Orbitron-Bold");
        bodyFont = Resources.Load<Font>("Fonts/Orbitron-Regular");

        // Camera fixed, looking at top of clock
        menuOrthoSize = 10f * GetPhoneZoom();
        var cam = Camera.main;
        cam.orthographicSize = menuOrthoSize;
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
        screenObjects = new GameObject[4];
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
        screenObjects[index] = screenObj;
    }

    void BuildTitleContent(Transform parent)
    {
        // Title text
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        titleGroup = titleObj.AddComponent<CanvasGroup>();
        var titleTextComp = titleObj.AddComponent<Text>();
        titleTextComp.font = titleFont;
        titleTextComp.text = "APOAPSIS II";
        titleTextComp.fontSize = 420;
        titleTextComp.fontStyle = FontStyle.Bold;
        titleTextComp.alignment = TextAnchor.MiddleCenter;
        titleTextComp.color = Color.white;
        titleTextComp.raycastTarget = false;
        titleTextComp.horizontalOverflow = HorizontalWrapMode.Overflow;
        titleTextComp.verticalOverflow = VerticalWrapMode.Overflow;
        titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(1800, 450);
        titleRect.anchoredPosition = new Vector2(0f, 150f);

        // Subtle glow via outline
        var outline = titleObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.4f, 0.6f, 1f, 0.35f);
        outline.effectDistance = new Vector2(4f, 4f);

        // Subtitle
        var subObj = new GameObject("Subtitle");
        subObj.transform.SetParent(parent, false);
        subtitleGroup = subObj.AddComponent<CanvasGroup>();
        var subText = subObj.AddComponent<Text>();
        subText.font = bodyFont;
        subText.text = "AN ORBITAL PUZZLE GAME";
        subText.fontSize = 52;
        subText.alignment = TextAnchor.MiddleCenter;
        subText.color = new Color(0.7f, 0.7f, 0.9f, 0.8f);
        subText.raycastTarget = false;
        subtitleRect = subObj.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.sizeDelta = new Vector2(800, 50);
        subtitleRect.anchoredPosition = new Vector2(0f, -30f);

        // Play button — goes to highest unlocked chapter
        CreatePlayButton(parent, new Vector2(0f, -180f), 200f, () =>
        {
            int target = 1;
            for (int ch = LevelRegistry.GetChapterCount(); ch >= 1; ch--)
            {
                if (LevelRegistry.IsLevelUnlocked(ch, 1))
                {
                    target = ch;
                    break;
                }
            }
            NavigateTo(target);
        });

        // Start fly-in animation
        titleGroup.alpha = 0f;
        subtitleGroup.alpha = 0f;
        titleRect.localScale = new Vector3(6f, 6f, 1f);
        titleAnimTimer = 0f;
        titleAnimating = true;
    }



    void CreatePlayButton(Transform parent, Vector2 pos, float size, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject("PlayBtn");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(size, size);
        btnRect.anchoredPosition = pos;

        // Circle background
        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = GenerateCircleSprite(new Color(0.15f, 0.25f, 0.5f));
        btnImg.color = new Color(1f, 1f, 1f, 0.9f);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        btn.colors = colors;
        btn.onClick.AddListener(action);

        // Circle border glow
        var ringObj = new GameObject("Ring");
        ringObj.transform.SetParent(btnObj.transform, false);
        var ringRect = ringObj.AddComponent<RectTransform>();
        ringRect.anchorMin = Vector2.zero;
        ringRect.anchorMax = Vector2.one;
        ringRect.sizeDelta = new Vector2(10, 10);
        var ringImg = ringObj.AddComponent<Image>();
        ringImg.sprite = GenerateCircleRing(new Color(0.4f, 0.6f, 1f));
        ringImg.raycastTarget = false;

        // Play triangle icon
        var triObj = new GameObject("PlayIcon");
        triObj.transform.SetParent(btnObj.transform, false);
        var triRect = triObj.AddComponent<RectTransform>();
        triRect.anchorMin = new Vector2(0.25f, 0.25f);
        triRect.anchorMax = new Vector2(0.75f, 0.75f);
        triRect.sizeDelta = Vector2.zero;
        var triImg = triObj.AddComponent<Image>();
        triImg.sprite = GeneratePlayTriangle();
        triImg.raycastTarget = false;
        triImg.color = Color.white;
    }

    void BuildChapterContent(Transform parent, int chapter)
    {
        // Chapter title
        var headerObj = new GameObject("Header");
        headerObj.transform.SetParent(parent, false);
        var headerText = headerObj.AddComponent<Text>();
        headerText.font = titleFont;
        headerText.text = $"CHAPTER {ToRoman(chapter)}";
        headerText.fontSize = 120;
        headerText.fontStyle = FontStyle.Bold;
        headerText.alignment = TextAnchor.MiddleCenter;
        headerText.color = Color.white;
        headerText.raycastTarget = false;
        headerText.horizontalOverflow = HorizontalWrapMode.Overflow;
        var headerRect = headerObj.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.5f, 0.5f);
        headerRect.anchorMax = new Vector2(0.5f, 0.5f);
        headerRect.sizeDelta = new Vector2(800, 80);
        headerRect.anchoredPosition = new Vector2(0f, 400f);

        // Level grid
        int count = LevelRegistry.GetLevelCount(chapter);
        int cols = 5;
        float spacing = 160f;
        float startX = -(cols - 1) * spacing / 2f;
        int rows = Mathf.CeilToInt((float)count / cols);
        float gridHeight = (rows - 1) * spacing;
        float startY = gridHeight / 2f;

        for (int i = 0; i < count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            CreateLevelButton(parent, chapter, i + 1,
                new Vector2(startX + col * spacing, startY - row * spacing));
        }

        // Left arrow — go back (to title or previous chapter)
        int prevStop = chapter - 1; // chapter 1 → stop 0 (title), chapter 2 → stop 1, etc.
        CreateArrowButton(parent, new Vector2(-480f, 0f), true, () => NavigateTo(prevStop));

        // Right arrow — go forward (to next chapter if it exists)
        if (chapter < LevelRegistry.GetChapterCount())
            CreateArrowButton(parent, new Vector2(480f, 0f), false, () => NavigateTo(chapter + 1));
    }

    void CreateArrowButton(Transform parent, Vector2 pos, bool pointLeft, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject(pointLeft ? "ArrowLeft" : "ArrowRight");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(70, 70);
        btnRect.anchoredPosition = pos;

        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = GenerateArrowSprite(pointLeft);
        btnImg.color = new Color(1f, 1f, 1f, 0.7f);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var colors = btn.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 0.7f);
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        btn.colors = colors;
        btn.onClick.AddListener(action);
    }

    Sprite GenerateArrowSprite(bool pointLeft)
    {
        int res = 64;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = res / 2f;

        // Chevron arrow: > or <
        Vector2 tip, top, bot;
        if (pointLeft)
        {
            tip = new Vector2(res * 0.25f, half);
            top = new Vector2(res * 0.75f, res * 0.85f);
            bot = new Vector2(res * 0.75f, res * 0.15f);
        }
        else
        {
            tip = new Vector2(res * 0.75f, half);
            top = new Vector2(res * 0.25f, res * 0.85f);
            bot = new Vector2(res * 0.25f, res * 0.15f);
        }

        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInTriangle(p, tip, top, bot))
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    void CreateLevelButton(Transform parent, int chapter, int level, Vector2 pos)
    {
        var config = LevelRegistry.GetLevel(chapter, level);
        if (config == null) return;

        var pc = config.planet;
        bool unlocked = LevelRegistry.IsLevelUnlocked(chapter, level);

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
        btn.interactable = unlocked;

        if (unlocked)
        {
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.3f, 1.3f, 1.3f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            btn.colors = colors;
            btn.onClick.AddListener(() =>
            {
                SceneBootstrap.LoadLevel(chapter, level);
            });
        }
        else
        {
            // Greyed out locked appearance
            var colors = btn.colors;
            colors.disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            btn.colors = colors;
            btnImg.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }

        // Atmosphere ring using the planet's atmosphere color
        var ringObj = new GameObject("Ring");
        ringObj.transform.SetParent(btnObj.transform, false);
        var ringRect = ringObj.AddComponent<RectTransform>();
        ringRect.anchorMin = Vector2.zero;
        ringRect.anchorMax = Vector2.one;
        ringRect.sizeDelta = new Vector2(14, 14);
        var ringImg = ringObj.AddComponent<Image>();
        ringImg.sprite = GenerateCircleRing(unlocked ? pc.atmosphereColor : new Color(0.3f, 0.3f, 0.3f));
        ringImg.color = new Color(1f, 1f, 1f, unlocked ? 0.5f : 0.2f);
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
        lbl.color = unlocked ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.5f);
        lbl.raycastTarget = false;
        var lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;

        // High score and perfect star below the planet
        if (LevelRegistry.GetHighScore(chapter, level) > 0 || LevelRegistry.IsPerfect(chapter, level))
        {
            int highScore = LevelRegistry.GetHighScore(chapter, level);
            bool perfect = LevelRegistry.IsPerfect(chapter, level);

            // Score + star in one line below the planet
            var infoObj = new GameObject("LevelInfo");
            infoObj.transform.SetParent(btnObj.transform, false);
            var infoRect = infoObj.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0.5f, 0f);
            infoRect.anchorMax = new Vector2(0.5f, 0f);
            infoRect.sizeDelta = new Vector2(140, 30);
            infoRect.anchoredPosition = new Vector2(0f, -20f);

            // Score text first, then star after
            if (highScore > 0)
            {
                var scoreTxtObj = new GameObject("Score");
                scoreTxtObj.transform.SetParent(infoObj.transform, false);
                var scoreTxt = scoreTxtObj.AddComponent<Text>();
                scoreTxt.font = titleFont;
                scoreTxt.text = highScore.ToString();
                scoreTxt.fontSize = 25;
                scoreTxt.alignment = TextAnchor.MiddleCenter;
                scoreTxt.color = new Color(1f, 0.85f, 0.2f, 0.9f);
                scoreTxt.raycastTarget = false;
                scoreTxt.horizontalOverflow = HorizontalWrapMode.Overflow;
                scoreTxt.verticalOverflow = VerticalWrapMode.Overflow;
                var scoreRect = scoreTxtObj.GetComponent<RectTransform>();
                scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
                scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
                scoreRect.sizeDelta = new Vector2(80, 30);
                scoreRect.anchoredPosition = perfect ? new Vector2(-12f, 0f) : Vector2.zero;
            }

            // Star icon after the score
            if (perfect)
            {
                var starObj = new GameObject("PerfectStar");
                starObj.transform.SetParent(infoObj.transform, false);
                var starImg = starObj.AddComponent<Image>();
                starImg.sprite = GenerateStarSprite();
                starImg.color = new Color(1f, 0.85f, 0.2f);
                starImg.raycastTarget = false;
                var starRect = starObj.GetComponent<RectTransform>();
                starRect.anchorMin = new Vector2(0.5f, 0.5f);
                starRect.anchorMax = new Vector2(0.5f, 0.5f);
                starRect.sizeDelta = new Vector2(16, 16);
                starRect.anchoredPosition = highScore > 0 ? new Vector2(32f, 0f) : Vector2.zero;
            }
        }

        // Gentle bob animation
        var bob = btnObj.AddComponent<UIBob>();
        bob.amplitude = 5f;
        bob.speed = 1.2f;
        bob.phaseOffset = level * 0.7f;
    }

    // --- NAVIGATION ---
    // Rotate CW to bring next screen up: each stop is -90° further
    public void NavigateTo(int stop)
    {
        if (rotating || stop == currentStop) return;
        float to = stop * 90f;
        float delta = Mathf.DeltaAngle(currentAngle, to);

        // From title (stop 0) to any chapter: always go clockwise (positive)
        if (currentStop == 0 && stop > 0 && delta < 0f)
            delta += 360f;

        targetAngle = currentAngle + delta;
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
        // Clock rotation
        if (rotating)
        {
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

        // Swipe navigation
        HandleSwipe();

        // Title fly-in animation
        if (titleAnimating && titleRect != null)
        {
            // Cap delta to prevent animation completing in one frame after loading
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            titleAnimTimer += dt;
            float t = Mathf.Clamp01(titleAnimTimer / titleAnimDuration);
            // Strong ease-out: fast start, gentle landing
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            // Scale: 6 → 2, alpha: 0 → 1
            float scale = Mathf.Lerp(6f, 2f, eased);
            titleRect.localScale = new Vector3(scale, scale, 1f);
            // Alpha fades in faster (ease-in-out)
            float alphaT = Mathf.Clamp01(titleAnimTimer / (titleAnimDuration * 0.6f));
            titleGroup.alpha = EaseInOut(alphaT);

            // Subtitle fades in after title mostly settled
            float subT = Mathf.Clamp01((titleAnimTimer - 1.5f) / 0.8f);
            subtitleGroup.alpha = EaseInOut(subT);

            if (t >= 1f)
                titleAnimating = false;
        }
    }

    void HandleSwipe()
    {
        bool pressed = false;
        bool justPressed = false;
        bool justReleased = false;
        Vector2 pos = Vector2.zero;

        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            pressed = touch.press.isPressed;
            justPressed = touch.press.wasPressedThisFrame;
            justReleased = touch.press.wasReleasedThisFrame;
            pos = touch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            pressed = Mouse.current.leftButton.isPressed;
            justPressed = Mouse.current.leftButton.wasPressedThisFrame;
            justReleased = Mouse.current.leftButton.wasReleasedThisFrame;
            pos = Mouse.current.position.ReadValue();
        }

        if (justPressed)
        {
            swipeTracking = true;
            swipeStartPos = pos;
        }
        else if (justReleased && swipeTracking)
        {
            swipeTracking = false;
            Vector2 delta = pos - swipeStartPos;

            // Must be primarily horizontal and exceed minimum distance
            if (Mathf.Abs(delta.x) > swipeMinDistance && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                int maxStop = LevelRegistry.GetChapterCount();
                if (delta.x < 0) // swipe left → next
                {
                    if (currentStop < maxStop)
                        NavigateTo(currentStop + 1);
                }
                else // swipe right → previous
                {
                    if (currentStop > 0)
                        NavigateTo(currentStop - 1);
                }
            }
        }
        else if (!pressed)
        {
            swipeTracking = false;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        clockPivot.SetActive(true);

        var cam = Camera.main;
        cam.transform.position = new Vector3(0f, clockRadius, -10f);
        cam.transform.rotation = Quaternion.identity;
        cam.orthographicSize = menuOrthoSize;

        clockPivot.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        // Restart title fly-in if returning to title screen
        if (currentStop == 0 && titleRect != null)
        {
            titleGroup.alpha = 0f;
            subtitleGroup.alpha = 0f;
            titleRect.localScale = new Vector3(6f, 6f, 1f);
            titleAnimTimer = 0f;
            titleAnimating = true;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        clockPivot.SetActive(false);
    }

    public void ReturnToLevelSelect()
    {
        int stop = LevelRegistry.CurrentChapter;

        // Rebuild the chapter screen to refresh unlock states
        if (screenObjects[stop] != null)
        {
            var oldPos = screenObjects[stop].transform.localPosition;
            var oldRot = screenObjects[stop].transform.localRotation;
            Object.Destroy(screenObjects[stop]);
            BuildScreen(stop, oldPos, oldRot.eulerAngles.z, (parent) => BuildChapterContent(parent, stop));
        }

        Show();
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

        // Fallback: generate a simple gradient circle (works on all platforms)
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        int half = res / 2;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float d = Mathf.Sqrt((x - half) * (x - half) + (y - half) * (y - half)) / half;
                if (d > 1f)
                    tex.SetPixel(x, y, Color.clear);
                else
                {
                    Color c = Color.Lerp(coreColor, rimColor, d * d);
                    // Rim glow
                    float rim = Mathf.Pow(d, 2.5f);
                    c = Color.Lerp(c, atmoColor, rim * 0.6f);
                    c.a = 1f - Mathf.Clamp01((d - 0.95f) / 0.05f);
                    tex.SetPixel(x, y, c);
                }
            }
        tex.Apply();

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

    Sprite GeneratePlayTriangle()
    {
        int res = 64;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = res / 2f;

        // Triangle pointing right, slightly offset right for visual centering
        Vector2 a = new Vector2(res * 0.3f, res * 0.15f);  // bottom-left
        Vector2 b = new Vector2(res * 0.3f, res * 0.85f);  // top-left
        Vector2 c = new Vector2(res * 0.8f, half);          // right point

        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                Vector2 p = new Vector2(x, y);
                if (PointInTriangle(p, a, b, c))
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }

    static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float d1 = (p.x - b.x) * (a.y - b.y) - (a.x - b.x) * (p.y - b.y);
        float d2 = (p.x - c.x) * (b.y - c.y) - (b.x - c.x) * (p.y - c.y);
        float d3 = (p.x - a.x) * (c.y - a.y) - (c.x - a.x) * (p.y - a.y);
        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(hasNeg && hasPos);
    }

    Sprite _starSprite;
    Sprite GenerateStarSprite()
    {
        if (_starSprite != null) return _starSprite;
        int res = 32;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float half = res / 2f;

        // 5-pointed star
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float px = (x - half) / half;
                float py = (y - half) / half;
                float angle = Mathf.Atan2(py, px);
                float dist = Mathf.Sqrt(px * px + py * py);
                // Star shape: alternating radius
                float starAngle = angle + Mathf.PI / 2f;
                float r = 0.45f + 0.45f * Mathf.Cos(5f * starAngle);
                tex.SetPixel(x, y, dist < r ? Color.white : Color.clear);
            }
        tex.Apply();
        _starSprite = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
        return _starSprite;
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
