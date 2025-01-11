using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Light mainLight; // The main light source in the scene
    public LayerMask shadowCastingLayers; // Layers to check for shadow casting objects
    public bool isInShadow = false; // Indicates whether the object is in shadow

    void Start()
    {
        if (mainLight == null)
        {
            // Automatically find the main light source
            mainLight = FindObjectOfType<Light>();
            if (mainLight == null)
            {
                Debug.LogError("No Light found in the scene!");
                return;
            }
        }
    }

    void Update()
    {
        DetectShadow();
    }

    void DetectShadow()
    {
        // Get the direction of the light
        Vector3 lightDirection = -mainLight.transform.forward;

        // Cast a ray from the object's position in the direction of the light
        Ray ray = new Ray(transform.position, lightDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, shadowCastingLayers))
        {
            // If the ray hits another object, the object is in shadow
            isInShadow = true;
        }
        else
        {
            // If no object blocks the ray, the object is not in shadow
            isInShadow = false;
        }

        Debug.Log($"Is in Shadow: {isInShadow}");
    }
}
