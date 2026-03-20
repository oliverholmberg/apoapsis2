using UnityEngine;

public class WorldRotation : MonoBehaviour
{
    Transform cam;
    Vector3 pivot;

    void Start()
    {
        cam = Camera.main.transform;
        pivot = transform.position;
    }

    void Update()
    {
        if (InputManager.Instance == null) return;

        float angle = InputManager.Instance.DragDelta;
        if (angle == 0f) return;

        // Rotate camera around planet center
        cam.RotateAround(pivot, Vector3.forward, angle);

        // Pre-launch: rotate rocket with the world
        var rc = Object.FindFirstObjectByType<RocketController>();
        if (rc != null && !rc.HasLaunched)
        {
            var rocket = rc.gameObject;
            Vector3 offset = rocket.transform.position - pivot;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            rocket.transform.position = pivot + rot * offset;
            rocket.transform.rotation *= rot;
        }
    }
}
