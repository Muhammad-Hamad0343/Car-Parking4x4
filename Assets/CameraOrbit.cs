using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Target and Camera Settings")]
    public Transform target;       // The target object to orbit around
    public float distance = 5.0f;  // Distance from the target
    public Vector3 offset;//= Vector3.zero; // Offset for the target position in world space

    [Header("Orbit Settings")]
    public float rotationSpeed = 100.0f;
    public float minYAngle = -20f;
    public float maxYAngle = 80f;

    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        // Set initial camera angles directly facing the target
        Vector3 initialOffset = transform.position - (target.position + offset);
        currentX = Vector3.SignedAngle(Vector3.forward, new Vector3(initialOffset.x, 0, initialOffset.z), Vector3.up);

        currentY = Mathf.Atan2(initialOffset.y, initialOffset.magnitude) * Mathf.Rad2Deg;

        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        // Optional: Unlock cursor for better camera control
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Handle mouse click and drag or touch drag input
        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                currentX += touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                currentY -= touch.deltaPosition.y * rotationSpeed * Time.deltaTime;
            }
        }

        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
    }

    void LateUpdate()
    {
        // Compute new camera position and rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distance) + (target.position + offset);

        transform.position = position;
        transform.LookAt(target.position + offset);
    }
}
