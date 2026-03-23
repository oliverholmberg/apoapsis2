using UnityEngine;

public class UIBob : MonoBehaviour
{
    public float amplitude = 5f;
    public float speed = 1.2f;
    public float phaseOffset;

    RectTransform rect;
    Vector2 basePos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.unscaledTime * speed + phaseOffset) * amplitude;
        rect.anchoredPosition = basePos + new Vector2(0f, offset);
    }
}
