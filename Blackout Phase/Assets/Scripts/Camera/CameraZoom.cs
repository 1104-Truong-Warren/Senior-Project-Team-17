// Warren
using UnityEngine;

// Resource: https://youtu.be/pJQndtJ2rk0?si=1g5IUdCvv3ocOUV1

// This script is used for camera control. The player can zoom in and out using the mouse scroll wheel. They can also move around the camera by holding the right mouse button.
public class CameraControl : MonoBehaviour
{
    // Created labled section which will show up in the inspector, allowing adjustments of zoom speed, zoom level, and sensitivity of the mouse wheel.
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minOrtho = 1f;
    public float maxOrtho = 20f;
    public float scrollSensitivity = 1f;

    // Created labled section of panning settings.
    [Header("Pan Settings")]
    public float panSpeed = 5f;
    
    private Camera cam;
    private float targetOrtho;
    private Vector3 dragOrigin;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>(); // Reference to the Camera component game object.
        // Sets initial target zoom to current camera zoom.
        if (cam != null)
        {
            targetOrtho = cam.orthographicSize;
            startPosition = transform.position; 
            targetPosition = startPosition;
        }
        else
        {
            Debug.LogError("Camera component not found!"); // If no camera is found, the the script will be disabled.
            enabled = false;
        }
    }

    // Calls zoom and pan handling methods.
    void Update()
    {
        HandleZoom();
        HandlePan();

        // Resource: https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panSpeed); // Resets the camera's position.
    }

    // Function that gets the mouse scroll wheel input, where -1 is down, 1 is up, and 0 is no scroll.
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            // Adjust the target orthographic size
            targetOrtho -= scroll * scrollSensitivity;

            // Resource: https://docs.unity3d.com/ScriptReference/Mathf.Clamp.html
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho); // Ensures that zoom stays within min and max bounds.
        }

        // Resource: https://docs.unity3d.com/ScriptReference/Mathf.MoveTowards.html
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetOrtho, zoomSpeed * Time.deltaTime * Mathf.Abs(targetOrtho)); // Smoothly changes camera zoom toward target, Math.MoveTowards moves toward target at constant speed, and Mathf.Abs(targetOrtho) is the zoom speed that scales with zoom level.
    }

    // Function where with you hold or press right click, it store the curren mouse position in world coordinates.
    void HandlePan()
    {
        // Check for right mouse button down
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition); // Resource: https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html
        }

        // Check for right mouse button held down
        if (Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);  // Calculate the difference between the current mouse position and the origin point
            transform.position += difference; // Move the camera by that difference
            targetPosition = transform.position; // When panning, the current camera position is in the target position 
        }

        // Check for right mouse button release
        if (Input.GetMouseButtonUp(1))
        {
            targetPosition = startPosition; // When released, set the target position back to the original start position
        }
    }
}