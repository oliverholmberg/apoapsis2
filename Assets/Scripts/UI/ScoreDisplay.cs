using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public static ScoreDisplay Instance { get; private set; }

    public int score;
    Font font;
    GUIStyle scoreStyle;
    GUIStyle popStyle;
    GUIStyle levelStyle;

    // Pop-up feedback
    string popText = "";
    float popTimer;
    float popDuration = 1.2f;
    Vector2 popScreenPos;

    void Awake()
    {
        Instance = this;
        font = Resources.Load<Font>("Fonts/Orbitron-Bold");
    }

    public void AddScore(int points, Vector3 worldPos)
    {
        score += points;

        // Show floating score pop-up
        var cam = Camera.main;
        if (cam != null)
            popScreenPos = cam.WorldToScreenPoint(worldPos);
        popText = $"+{points}";
        popTimer = popDuration;

        NotifyOverlay();
    }

    public void AddScore(int points)
    {
        score += points;
        NotifyOverlay();
    }

    void NotifyOverlay()
    {
        var overlay = Object.FindFirstObjectByType<StateOverlay>();
        if (overlay != null)
            overlay.UpdateScore(score);
    }

    void OnGUI()
    {
        if (scoreStyle == null)
        {
            // Scale font sizes for high-DPI screens (capped to avoid oversized text)
            float uiScale = Mathf.Clamp(Screen.dpi / 160f, 1f, 1.8f);

            scoreStyle = new GUIStyle
            {
                font = font,
                fontSize = (int)(32 * uiScale),
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold
            };
            scoreStyle.normal.textColor = new Color(1f, 1f, 1f, 0.8f);

            popStyle = new GUIStyle
            {
                font = font,
                fontSize = (int)(22 * uiScale),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            levelStyle = new GUIStyle
            {
                font = font,
                fontSize = (int)(24 * uiScale),
                alignment = TextAnchor.UpperRight,
                fontStyle = FontStyle.Bold
            };
            levelStyle.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
        }

        // Safe area inset for notch/rounded corners
        Rect safe = Screen.safeArea;
        float topInset = safe.y;
        float rightInset = Screen.width - safe.xMax;

        // Level indicator at top right
        string levelText = $"{LevelRegistry.CurrentChapter}-{LevelRegistry.CurrentLevel}";
        GUI.Label(new Rect(Screen.width - 130 - rightInset, topInset + 10, 115, 40), levelText, levelStyle);

        // Score at top center
        GUI.Label(new Rect(Screen.width / 2f - 150, topInset + 10, 300, 50), score.ToString(), scoreStyle);

        // Floating pop-up
        if (popTimer > 0f)
        {
            popTimer -= Time.unscaledDeltaTime;
            float t = 1f - (popTimer / popDuration);
            float alpha = Mathf.Lerp(1f, 0f, t);
            float yOffset = Mathf.Lerp(0f, -40f, t);

            popStyle.normal.textColor = new Color(1f, 0.9f, 0.3f, alpha);
            float screenY = Screen.height - popScreenPos.y; // GUI y is inverted
            GUI.Label(new Rect(popScreenPos.x - 60, screenY + yOffset - 30, 120, 40), popText, popStyle);
        }
    }
}
