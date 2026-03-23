using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // Thrust zone: bottom 30% of screen, left or right 35%
    public float thrustZoneHeight = 0.3f;
    public float thrustZoneWidth = 0.35f;

    // Drag
    public float dragSensitivity = 0.15f;

    public bool IsThrustingTouch { get; private set; }
    public float DragDelta { get; private set; }

    bool isDragging;
    Vector2 lastDragPos;
    bool touchStartedInThrustZone;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        IsThrustingTouch = false;
        DragDelta = 0f;

        // Keyboard — always works
        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) DragDelta += 1f;
            if (Keyboard.current.rightArrowKey.isPressed) DragDelta -= 1f;
            DragDelta *= 60f * Time.deltaTime;
        }

        // Touch
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 pos = Touchscreen.current.primaryTouch.position.ReadValue();
            HandlePointer(pos, Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
        }
        // Mouse fallback
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            HandlePointer(pos, Mouse.current.leftButton.wasPressedThisFrame);
        }
        else
        {
            isDragging = false;
            touchStartedInThrustZone = false;
        }
    }

    void HandlePointer(Vector2 screenPos, bool justPressed)
    {
        float nx = screenPos.x / Screen.width;
        float ny = screenPos.y / Screen.height;

        bool inThrustZone = ny < thrustZoneHeight && (nx < thrustZoneWidth || nx > (1f - thrustZoneWidth));

        if (justPressed)
        {
            touchStartedInThrustZone = inThrustZone;
            isDragging = !inThrustZone;
            lastDragPos = screenPos;
        }

        if (touchStartedInThrustZone)
        {
            IsThrustingTouch = true;
        }
        else if (isDragging)
        {
            float dx = screenPos.x - lastDragPos.x;
            DragDelta += dx * dragSensitivity;
            lastDragPos = screenPos;
        }
    }

    public bool IsThrusting()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) return true;
        return IsThrustingTouch;
    }
}
