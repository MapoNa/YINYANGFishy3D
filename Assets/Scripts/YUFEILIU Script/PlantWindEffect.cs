using UnityEngine;

public class PlantWindEffect : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("Wind direction in degrees")]
    [Range(-360f, 360f)]
    public float windDirectionDegrees = 0f;

    [Tooltip("Wind strength")]
    [Range(0f, 100f)]
    public float windStrength = 1f;

    [Tooltip("Frequency of wind noise")]
    public float windNoiseFrequency = 1f;

    [Tooltip("Amplitude of wind noise")]
    public float windNoiseAmplitude = 1f;

    [Tooltip("Seed for wind variation")]
    public float windSeed = 0f;

    private Vector3 initialPosition; // Initial position of the plant
    private Quaternion initialRotation; // Initial rotation of the plant

    private Renderer objectRenderer; // Renderer component of the object
    private Vector3 rootPosition; // Position of the plant's root
    private float objectHeight; // Height of the plant

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Retrieve the Renderer component of the object
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // Calculate the root position and height of the plant
            rootPosition = objectRenderer.bounds.min; // Lowest point of the object
            objectHeight = objectRenderer.bounds.size.y; // Height of the object
        }
        else
        {
            Debug.LogWarning("No Renderer found on the object. Wind effect may not behave as expected.");
            rootPosition = transform.position;
            objectHeight = 1f; // Default height
        }

        // Randomize the wind seed for each plant
        windSeed = Random.Range(-10f, 10f);
    }

    void Update()
    {
        SimulateWind();
    }

    private void SimulateWind()
    {
        // Generate wind variation based on Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * windNoiseFrequency + windSeed, 0f) * windNoiseAmplitude;

        // Calculate the wind direction vector
        float radians = (windDirectionDegrees + noise) * Mathf.Deg2Rad;
        Vector3 windDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)).normalized;

        // Determine the height influence factor based on the object's position relative to its root
        float heightInfluence = Mathf.Clamp01((transform.position.y - rootPosition.y) / objectHeight);

        // Adjust sway amount based on height influence (top sways the most, root remains static)
        float swayAmount = windStrength * noise * heightInfluence;

        // Apply the swaying effect, with maximum effect at the top and minimal effect at the root
        transform.rotation = initialRotation * Quaternion.Euler(
            swayAmount * windDirection.z,
            0,
            swayAmount * windDirection.x
        );
    }
}
