using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    GameObject panel;
    bool paused;
    Font titleFont;
    Font bodyFont;

    // OnGUI text
    string title = "PAUSED";
    GUIStyle titleStyle;

    public bool IsPaused => paused;

    public void Initialize()
    {
        titleFont = Resources.Load<Font>("Fonts/Orbitron-Bold");
        bodyFont = Resources.Load<Font>("Fonts/Orbitron-Regular");

        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 110;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        gameObject.AddComponent<GraphicRaycaster>();

        // Pause icon button (always visible, outside panel)
        var iconObj = new GameObject("PauseIcon");
        iconObj.transform.SetParent(transform, false);
        var iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 1f);
        iconRect.anchorMax = new Vector2(0f, 1f);
        iconRect.pivot = new Vector2(0f, 1f);
        iconRect.anchoredPosition = new Vector2(55f, -50f);
        iconRect.sizeDelta = new Vector2(55, 55);
        var iconImg = iconObj.AddComponent<Image>();
        iconImg.sprite = GeneratePauseIcon();
        iconImg.color = new Color(1f, 1f, 1f, 0.3f);
        var iconBtn = iconObj.AddComponent<Button>();
        iconBtn.targetGraphic = iconImg;
        var iconColors = iconBtn.colors;
        iconColors.normalColor = new Color(1f, 1f, 1f, 0.3f);
        iconColors.highlightedColor = new Color(1f, 1f, 1f, 0.5f);
        iconColors.pressedColor = new Color(1f, 1f, 1f, 0.7f);
        iconBtn.colors = iconColors;
        iconBtn.onClick.AddListener(() => Pause());

        // Darkener
        panel = new GameObject("Panel");
        panel.transform.SetParent(transform, false);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0.02f, 0.6f);

        group = panel.AddComponent<CanvasGroup>();
        group.alpha = 1f;

        // Glow
        var glowObj = new GameObject("Glow");
        glowObj.transform.SetParent(panel.transform, false);
        var glowRect = glowObj.AddComponent<RectTransform>();
        glowRect.anchorMin = new Vector2(0.5f, 0.5f);
        glowRect.anchorMax = new Vector2(0.5f, 0.5f);
        glowRect.sizeDelta = new Vector2(520, 420);
        var glowImg = glowObj.AddComponent<Image>();
        glowImg.sprite = UIHelper.GenerateRoundedRect(128, 64, 24, true);
        glowImg.type = Image.Type.Sliced;
        glowImg.color = new Color(0f, 0.6f, 1f, 0.1f);

        // Box
        var box = new GameObject("Box");
        box.transform.SetParent(panel.transform, false);
        var boxRect = box.AddComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(450, 340);
        var boxImg = box.AddComponent<Image>();
        boxImg.sprite = UIHelper.GenerateRoundedRect(128, 64, 20, false);
        boxImg.type = Image.Type.Sliced;
        boxImg.color = new Color(0.03f, 0.03f, 0.1f, 0.9f);

        // Border
        var borderObj = new GameObject("Border");
        borderObj.transform.SetParent(box.transform, false);
        var borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = Vector2.zero;
        var borderImg = borderObj.AddComponent<Image>();
        borderImg.sprite = UIHelper.GenerateRoundedRectBorder(128, 64, 20, 3);
        borderImg.type = Image.Type.Sliced;
        borderImg.color = new Color(0f, 0.6f, 1f, 0.3f);

        // Buttons
        CreateButton(box.transform, "RESUME", new Vector2(0f, 10f), new Color(0.1f, 0.5f, 0.3f, 0.9f), () => Resume());
        CreateButton(box.transform, "RESTART", new Vector2(0f, -60f), new Color(0.1f, 0.4f, 0.8f, 0.9f), () => { Resume(); SceneBootstrap.Reset(); });
        CreateButton(box.transform, "QUIT", new Vector2(0f, -130f), new Color(0.5f, 0.15f, 0.15f, 0.9f), () => { Resume(); SceneBootstrap.ShowMenu(); });

        panel.SetActive(false);
    }

    void CreateButton(Transform parent, string label, Vector2 pos, Color color, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject(label + "Btn");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = new Vector2(280, 55);
        btnRect.anchoredPosition = pos;

        var btnImg = btnObj.AddComponent<Image>();
        btnImg.sprite = UIHelper.GenerateRoundedRect(64, 32, 12, false);
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
        bdrImg.color = new Color(colors.highlightedColor.r, colors.highlightedColor.g, colors.highlightedColor.b, 0.4f);
        bdrImg.raycastTarget = false;

        // Label
        var lblObj = new GameObject("Label");
        lblObj.transform.SetParent(btnObj.transform, false);
        var lbl = lblObj.AddComponent<Text>();
        lbl.font = bodyFont;
        lbl.text = label;
        lbl.fontSize = 24;
        lbl.fontStyle = FontStyle.Bold;
        lbl.alignment = TextAnchor.MiddleCenter;
        lbl.color = Color.white;
        lbl.raycastTarget = false;
        var lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        paused = true;
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        paused = false;
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnGUI()
    {
        if (!paused) return;

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle
            {
                font = titleFont,
                fontSize = 48,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            titleStyle.normal.textColor = new Color(0.6f, 0.7f, 1f);
        }

        float cx = Screen.width / 2f;
        float cy = Screen.height / 2f;
        GUI.Label(new Rect(cx - 300, cy - 155, 600, 70), title, titleStyle);
    }

    Sprite GeneratePauseIcon()
    {
        int size = 32;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color[] clear = new Color[size * size];
        tex.SetPixels(clear);

        int barWidth = 6;
        int barHeight = 22;
        int gap = 4;
        int startY = (size - barHeight) / 2;
        int leftX = (size - barWidth * 2 - gap) / 2;
        int rightX = leftX + barWidth + gap;

        for (int y = startY; y < startY + barHeight; y++)
        {
            for (int x = leftX; x < leftX + barWidth; x++)
                tex.SetPixel(x, y, Color.white);
            for (int x = rightX; x < rightX + barWidth; x++)
                tex.SetPixel(x, y, Color.white);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }
}
