using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Player's Transform
    public Vector3 offset = new Vector3(0, 4, -2); // Offset from the player
    public float smoothSpeed = 0.3f; // Smoothing speed for movement and rotation
    private FishController fishController; // Reference to the FishController script

    private bool previousEatingState = false; // Tracks the previous state of isEating
    private Camera cameraComponent; // Reference to the Camera component
    public float zoomOutSize = 40f; // Camera size when isEating is true
    public float normalSize = 60f; // Camera size when isEating is false
    public float minSize = 20f; // Camera size when yinYangValue reaches the limit
    public float zoomSpeed = 2f; // Speed of camera size transition
    public float resizeSmoothSpeed = 0.2f; // Speed of camera resizing

    private void Start()
    {
        // Automatically find and assign the FishController script
        if (player != null)
        {
            fishController = player.GetComponent<FishController>();
            if (fishController == null)
            {
                Debug.LogError("FishController script not found on the player!");
            }
        }

        // Get the Camera component attached to this GameObject
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError("Camera component not found on this game object.");
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Target position
            Vector3 desiredPosition = player.position + offset;

            // Smoothly transition to the target position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);

            // Smoothly rotate to face the player
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.fixedDeltaTime);
        }

        // Handle eating camera feedback
        //HandleEatingFeedback();

        // Handle yinYangValue-based camera resizing
        HandleYinYangFeedback();
    }

    private void HandleEatingFeedback()
    {
        if (fishController == null || cameraComponent == null)
            return;

        // Detect state change
        if (fishController.isEating != previousEatingState)
        {
            if (fishController.isEating)
            {
                // isEating changed to true, zoom out quickly
                cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, zoomOutSize, zoomSpeed * Time.deltaTime);
            }
            else
            {
                // isEating changed to false, zoom in quickly
                cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, normalSize, zoomSpeed * Time.deltaTime);
            }

            // Update the previous state
            previousEatingState = fishController.isEating;
        }
    }

    private void HandleYinYangFeedback()
    {
        if (fishController == null || cameraComponent == null)
            return;

        if (Mathf.Abs(fishController.yinYangValue) >= 1f)
        {
            // When yinYangValue reaches the limit, shrink the camera size
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, minSize, resizeSmoothSpeed * Time.deltaTime);
        }
        else
        {
            // Restore the camera size to normal
            cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, normalSize, resizeSmoothSpeed * Time.deltaTime);
        }
    }
}
