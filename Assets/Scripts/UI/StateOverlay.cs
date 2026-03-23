using UnityEngine;
using UnityEngine.UI;

public class StateOverlay : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    Image glowImage;
    GameObject panel;

    float fadeTarget;
    float fadeSpeed = 3f;

    Font titleFont;
    Font bodyFont;
    GUIStyle titleStyle;
    GUIStyle subtitleStyle;
    GUIStyle statsStyle;

    string titleText = "";
    string subtitleText = "";
    string statsText = "";
    Color titleColor;
    Color statsColor;
    bool showing;

    // Pre-created buttons
    GameObject retryBtn;
    GameObject replayBtn;
    GameObject nextBtn;
    GameObject backBtn;

    public void Initialize()
    {
        titleFont = Resources.Load<Font>("Fonts/Orbitron-Bold");
        bodyFont = Resources.Load<Font>("Fonts/Orbitron-Regular");

        // Canvas for the box visuals only
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        gameObject.AddComponent<GraphicRaycaster>();

        // EventSystem needed for button clicks
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Full screen darkener
        panel = new GameObject("Panel");
        panel.transform.SetParent(transform, false);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0.02f, 0.5f);

        group = panel.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        // Glow behind box
        var glowObj = new GameObject("Glow");
        glowObj.transform.SetParent(panel.transform, false);
        var glowRect = glowObj.AddComponent<RectTransform>();
        glowRect.anchorMin = new Vector2(0.5f, 0.5f);
        glowRect.anchorMax = new Vector2(0.5f, 0.5f);
        glowRect.sizeDelta = new Vector2(720, 380);
        glowImage = glowObj.AddComponent<Image>();
        glowImage.sprite = UIHelper.GenerateRoundedRect(128, 64, 24, true);
        glowImage.type = Image.Type.Sliced;
        glowImage.color = new Color(0f, 0.8f, 1f, 0.15f);

        // Rounded box
        var boxGO = new GameObject("Box");
        boxGO.transform.SetParent(panel.transform, false);
        var boxRect = boxGO.AddComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(650, 300);
        var boxImg = boxGO.AddComponent<Image>();
        boxImg.sprite = UIHelper.GenerateRoundedRect(128, 64, 20, false);
        boxImg.type = Image.Type.Sliced;
        boxImg.color = new Color(0.03f, 0.03f, 0.1f, 0.85f);

        // Border
        var borderObj = new GameObject("Border");
        borderObj.transform.SetParent(boxGO.transform, false);
        var borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = Vector2.zero;
        var borderImg = borderObj.AddComponent<Image>();
        borderImg.sprite = UIHelper.GenerateRoundedRectBorder(128, 64, 20, 3);
        borderImg.type = Image.Type.Sliced;
        borderImg.color = new Color(0f, 0.8f, 1f, 0.4f);

        // Create all buttons upfront while panel is active
        retryBtn = MakeButton("RETRY", new Vector2(0f, -75f), boxGO.transform, () => SceneBootstrap.Reset());
        replayBtn = MakeButton("REPLAY", new Vector2(-120f, -75f), boxGO.transform, () => SceneBootstrap.Reset());
        nextBtn = MakeButton("NEXT", new Vector2(120f, -75f), boxGO.transform, () =>
        {
            LevelRegistry.AdvanceLevel();
            SceneBootstrap.Reset(reverse: true);
        });
        backBtn = MakeButton("BACK", new Vector2(120f, -75f), boxGO.transform, () =>
        {
            SceneBootstrap.ShowMenu();
        });

        HideAllButtons();
        panel.SetActive(false);
    }

    GameObject MakeButton(string label, Vector2 position, Transform parent, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject(label + "Button");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(200, 55);
        btnRect.anchoredPosition = position;

        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = UIHelper.GenerateRoundedRect(64, 32, 12, false);
        btnImg.type = Image.Type.Sliced;
        btnImg.color = new Color(0.1f, 0.4f, 0.8f, 0.9f);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        var colors = btn.colors;
        colors.normalColor = new Color(0.1f, 0.4f, 0.8f, 0.9f);
        colors.highlightedColor = new Color(0.2f, 0.5f, 1f, 1f);
        colors.pressedColor = new Color(0.05f, 0.3f, 0.6f, 1f);
        btn.colors = colors;
        btn.onClick.AddListener(action);

        // Border
        var bdrObj = new GameObject("Border");
        bdrObj.transform.SetParent(btnObj.transform, false);
        var bdrRect = bdrObj.AddComponent<RectTransform>();
        bdrRect.anchorMin = Vector2.zero;
        bdrRect.anchorMax = Vector2.one;
        bdrRect.sizeDelta = Vector2.zero;
        var bdrImg = bdrObj.AddComponent<Image>();
        bdrImg.sprite = UIHelper.GenerateRoundedRectBorder(64, 32, 12, 2);
        bdrImg.type = Image.Type.Sliced;
        bdrImg.color = new Color(0.3f, 0.6f, 1f, 0.5f);
        bdrImg.raycastTarget = false;

        // Label
        var lblObj = new GameObject("Label");
        lblObj.transform.SetParent(btnObj.transform, false);
        var lblText = lblObj.AddComponent<Text>();
        lblText.font = bodyFont;
        lblText.text = label;
        lblText.fontSize = 24;
        lblText.fontStyle = FontStyle.Bold;
        lblText.alignment = TextAnchor.MiddleCenter;
        lblText.color = Color.white;
        lblText.raycastTarget = false;
        var lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;

        return btnObj;
    }

    void HideAllButtons()
    {
        retryBtn.SetActive(false);
        replayBtn.SetActive(false);
        nextBtn.SetActive(false);
        backBtn.SetActive(false);
    }

    void BuildStyles()
    {
        titleStyle = new GUIStyle
        {
            font = titleFont,
            fontSize = 52,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = titleColor;

        subtitleStyle = new GUIStyle
        {
            font = bodyFont,
            fontSize = 22,
            alignment = TextAnchor.MiddleCenter
        };
        subtitleStyle.normal.textColor = new Color(0.7f, 0.7f, 0.8f, 0.8f);

        statsStyle = new GUIStyle
        {
            font = bodyFont,
            fontSize = 28,
            alignment = TextAnchor.MiddleCenter
        };
        statsStyle.normal.textColor = statsColor;
    }

    public void UpdateScore(int score)
    {
        if (showing && titleText == "LEVEL COMPLETE")
            statsText = $"SCORE: {score}";
    }

    public void ShowWin(int score)
    {
        titleText = "LEVEL COMPLETE";
        titleColor = new Color(0.2f, 1f, 0.6f);
        glowImage.color = new Color(0.2f, 1f, 0.6f, 0.12f);
        subtitleText = "";
        statsText = $"SCORE: {score}";
        statsColor = new Color(1f, 0.85f, 0.2f);

        HideAllButtons();
        bool hasNext = LevelRegistry.GetLevel(LevelRegistry.CurrentChapter, LevelRegistry.CurrentLevel + 1) != null
                    || LevelRegistry.GetLevel(LevelRegistry.CurrentChapter + 1, 1) != null;

        replayBtn.SetActive(true);
        if (hasNext)
            nextBtn.SetActive(true);
        else
            backBtn.SetActive(true);

        FadeIn();
    }

    public void ShowCrash()
    {
        titleText = "CRASHED";
        titleColor = new Color(1f, 0.2f, 0.15f);
        glowImage.color = new Color(1f, 0.2f, 0.1f, 0.1f);
        subtitleText = "";
        statsText = "";
        statsColor = Color.clear;

        HideAllButtons();
        retryBtn.SetActive(true);

        FadeIn();
    }

    public void ShowLostInSpace()
    {
        titleText = "LOST IN SPACE";
        titleColor = new Color(0.4f, 0.5f, 1f);
        glowImage.color = new Color(0.3f, 0.4f, 1f, 0.12f);
        subtitleText = "";
        statsText = "";
        statsColor = Color.clear;

        HideAllButtons();
        retryBtn.SetActive(true);

        FadeIn();
    }

    public void Hide()
    {
        fadeTarget = 0f;
        showing = false;
        panel.SetActive(false);
    }

    void FadeIn()
    {
        panel.SetActive(true);
        group.alpha = 0f;
        fadeTarget = 1f;
        showing = true;
    }

    void Update()
    {
        if (group == null) return;
        if (Mathf.Abs(group.alpha - fadeTarget) > 0.01f)
            group.alpha = Mathf.Lerp(group.alpha, fadeTarget, Time.unscaledDeltaTime * fadeSpeed);
    }

    void OnGUI()
    {
        if (!showing || group.alpha < 0.01f) return;

        BuildStyles();

        // Apply same alpha as canvas group
        var prevColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, group.alpha);

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;
        float boxW = 620f;

        GUI.Label(new Rect(cx - boxW / 2, cy - 100, boxW, 80), titleText, titleStyle);

        if (!string.IsNullOrEmpty(statsText))
            GUI.Label(new Rect(cx - boxW / 2, cy - 20, boxW, 50), statsText, statsStyle);

        if (!string.IsNullOrEmpty(subtitleText))
            GUI.Label(new Rect(cx - boxW / 2, cy + 40, boxW, 40), subtitleText, subtitleStyle);

        GUI.color = prevColor;
    }
}
