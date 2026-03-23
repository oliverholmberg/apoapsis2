using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    GameObject overlayObj;
    Image overlayImage;

    // Fade state
    float fadeTimer;
    float fadeDuration;
    float fadeFrom;
    float fadeTo;
    float rotateSpeed;
    System.Action onFadeComplete;
    bool fading;
    bool transitioning;

    // Level zoom-in
    float zoomStartSize;
    float zoomTargetSize;
    float zoomTimer;
    float zoomDuration = 1.4f;
    bool zooming;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateOverlay();
    }

    void CreateOverlay()
    {
        overlayObj = new GameObject("TransitionOverlay");
        overlayObj.transform.SetParent(transform, false);
        var canvas = overlayObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        var imgObj = new GameObject("Fade");
        imgObj.transform.SetParent(overlayObj.transform, false);
        overlayImage = imgObj.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0f);
        overlayImage.raycastTarget = false;
        var rect = imgObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        overlayObj.SetActive(false);
    }

    static float EaseInOut(float t)
    {
        return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    }

    /// Fade to black with no rotation (for level select)
    public void FadeOut(System.Action action)
    {
        if (transitioning) return;
        transitioning = true;
        overlayObj.SetActive(true);
        StartFade(0f, 1f, 0.5f, 0f, () =>
        {
            action?.Invoke();
            StartFade(1f, 0f, 0.4f, 0f, FinishTransition);
        });
    }

    /// Fade to black with rotation (for retry/next/quit in gameplay)
    public void TransitionOut(System.Action action, float rotSpeed = 200f)
    {
        if (transitioning) return;
        transitioning = true;
        overlayObj.SetActive(true);
        StartFade(0f, 1f, 0.6f, rotSpeed, () =>
        {
            action?.Invoke();
            StartFade(1f, 0f, 0.5f, 0f, FinishTransition);
        });
    }

    /// Fade to black with opposite rotation (for leaving level to menu)
    public void TransitionOutReverse(System.Action action, float rotSpeed = -200f)
    {
        TransitionOut(action, rotSpeed);
    }

    void StartFade(float from, float to, float duration, float rot, System.Action onComplete)
    {
        fadeFrom = from;
        fadeTo = to;
        fadeDuration = duration;
        fadeTimer = 0f;
        rotateSpeed = rot;
        onFadeComplete = onComplete;
        fading = true;
        overlayImage.color = new Color(0f, 0f, 0f, from);
    }

    void FinishTransition()
    {
        overlayObj.SetActive(false);
        transitioning = false;
        fading = false;
    }

    /// Start a level zoom-in effect
    public void StartLevelZoom(float targetOrtho)
    {
        var cam = Camera.main;
        if (cam == null) return;
        zoomStartSize = targetOrtho * 2.5f;
        zoomTargetSize = targetOrtho;
        cam.orthographicSize = zoomStartSize;
        zoomTimer = 0f;
        zooming = true;
    }

    void Update()
    {
        // Fade with easing
        if (fading)
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            fadeTimer += dt;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);
            float eased = EaseInOut(t);
            float alpha = Mathf.Lerp(fadeFrom, fadeTo, eased);
            overlayImage.color = new Color(0f, 0f, 0f, alpha);

            // Rotate during fade — eased rotation that slows at the end
            if (rotateSpeed != 0f)
            {
                float rotAmount = rotateSpeed * dt * (1f - eased * 0.7f); // slows as fade progresses
                var cam = Camera.main;
                if (cam != null)
                {
                    var world = GameObject.Find("World");
                    if (world != null)
                    {
                        cam.transform.RotateAround(world.transform.position, Vector3.forward, rotAmount);
                    }
                    else if (MenuCarousel.Instance != null && MenuCarousel.Instance.ClockPivot != null
                        && MenuCarousel.Instance.ClockPivot.activeSelf)
                    {
                        MenuCarousel.Instance.ClockPivot.transform.Rotate(0f, 0f, rotAmount);
                    }
                }
            }

            if (t >= 1f)
            {
                fading = false;
                overlayImage.color = new Color(0f, 0f, 0f, fadeTo);
                var cb = onFadeComplete;
                onFadeComplete = null;
                cb?.Invoke();
            }
        }

        // Level zoom
        if (zooming)
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            zoomTimer += dt;
            float t = Mathf.Clamp01(zoomTimer / zoomDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            var cam = Camera.main;
            if (cam != null)
                cam.orthographicSize = Mathf.Lerp(zoomStartSize, zoomTargetSize, eased);
            if (t >= 1f)
                zooming = false;
        }
    }

    public bool IsTransitioning => transitioning;
}
