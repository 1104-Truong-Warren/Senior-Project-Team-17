using UnityEngine;

// This script is used for camera control. The player can zoom in and out using the mouse scroll wheel. They can also move around the camera by holding the right mouse button.
public class CameraControl : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minOrtho = 1f;
    public float maxOrtho = 20f;
    public float scrollSensitivity = 1f;

    [Header("Pan Settings")]
    public float panSpeed = 5f;
    
    private Camera cam;
    private float targetOrtho;
    private Vector3 dragOrigin;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            targetOrtho = cam.orthographicSize;
            startPosition = transform.position; 
            targetPosition = startPosition;
        }
        else
        {
            Debug.LogError("Camera component not found!");
            enabled = false;
        }
    }

    void Update()
    {
        HandleZoom();
        HandlePan();
        // Resets the camera's position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panSpeed); 
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse Scroll Wheel");

        if (scroll != 0.0f)
        {
            // Adjust the target orthographic size
            targetOrtho -= scroll * scrollSensitivity;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetOrtho, zoomSpeed * Time.deltaTime * Mathf.Abs(targetOrtho));
    }

    void HandlePan()
    {
        // Check for right mouse button down
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // Check for right mouse button held down
        if (Input.GetMouseButton(1))
        {
            // Calculate the difference between the current mouse position and the origin point
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            // Move the camera by that difference
            transform.position += difference;
            // When panning, the current camera position is in the target position
            targetPosition = transform.position; 
        }

        // Check for right mouse button release
        if (Input.GetMouseButtonUp(1))
        {
            // When released, set the target position back to the original start position
            targetPosition = startPosition;
        }
    }
}