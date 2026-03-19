using UnityEngine;
using UnityEngine.InputSystem;

public class WorldRotation : MonoBehaviour
{
    public float rotationSpeed = 60f;

    Transform cam;
    Vector3 pivot;

    void Start()
    {
        cam = Camera.main.transform;
        pivot = transform.position; // planet center
    }

    void Update()
    {
        float input = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) input += 1f;
            if (Keyboard.current.rightArrowKey.isPressed) input -= 1f;
        }

        if (input == 0f) return;

        float angle = input * rotationSpeed * Time.deltaTime;

        // Rotate camera around planet center
        cam.RotateAround(pivot, Vector3.forward, angle);

        // Pre-launch: rotate rocket with the world so player can aim
        var rocket = GameObject.FindWithTag("affectedByPlanetGravity");
        if (rocket != null)
        {
            var rc = rocket.GetComponent<RocketController>();
            if (rc != null && !rc.HasLaunched)
            {
                Vector3 offset = rocket.transform.position - pivot;
                Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                rocket.transform.position = pivot + rot * offset;
                rocket.transform.rotation *= rot;
            }
        }
    }
}
