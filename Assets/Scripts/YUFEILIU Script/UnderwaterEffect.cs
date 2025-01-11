using UnityEngine;

public class ManageAudioLowPassFilters : MonoBehaviour
{
    public GameObject[] targetObjects; // Array to store the target game objects
    private AudioSource[] audioSources; // Array to store AudioSource components
    public GameObject Player; // Player object, used to detect position
    public AudioSource Swin; // Audio source for Swin sound
    private Vector3 lastPlayerPosition; // Last recorded position of the player
    private float checkInterval = 0.5f; // Interval to check position (in seconds)
    private float timer = 0f; // Timer to track elapsed time

    private bool isLowPassEnabled = false; // Tracks if low-pass effect is enabled
    private bool isLowPassDisabled = true; // Tracks if low-pass effect is disabled

    public PauseManager PM; // Reference to PauseManager

    void Start()
    {
        if (PM == null)
        {
            PM = FindObjectOfType<PauseManager>();
            if (PM == null)
            {
                Debug.LogError("PauseManager not found in the scene!");
            }
        }

        // Initialize the AudioSource array with the same size as the target objects array
        audioSources = new AudioSource[targetObjects.Length];

        // Iterate through the target objects array and get the AudioSource component from each object
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null) // Ensure the object is not null
            {
                audioSources[i] = targetObjects[i].GetComponent<AudioSource>();
                if (audioSources[i] == null)
                {
                    Debug.LogWarning($"AudioSource not found on {targetObjects[i].name}");
                }
            }
        }

        if (Player != null)
        {
            lastPlayerPosition = Player.transform.position; // Initialize the last position
        }
    }

    void Update()
    {
        if (Player != null)
        {
            // Timer for position checking
            timer += Time.deltaTime;

            if (timer >= checkInterval)
            {
                // Check if the player's position has changed
                if (Player.transform.position == lastPlayerPosition)
                {
                    AdjustSwinVolume(false); // Reduce volume if position hasn't changed
                }
                else
                {
                    AdjustSwinVolume(true); // Maintain volume if position has changed
                    lastPlayerPosition = Player.transform.position; // Update last position
                }

                timer = 0f; // Reset timer
            }

            // Check if the player's Y-coordinate is below -1 or game is paused
            bool enableLowPass = Player.transform.position.y < -1 || (PM != null && PM.isPaused == true);
            SimulateLowPassFilters(enableLowPass); // Apply low-pass effect
            AdjustSwinLowPass(enableLowPass); // Adjust Swin low-pass effect separately
        }
    }

    private void SimulateLowPassFilters(bool enable)
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource != null && audioSource != Swin) // Exclude Swin from general low-pass logic
            {
                if (enable && !isLowPassEnabled) // Enable low-pass effect if not already enabled
                {
                    audioSource.volume *= 0.2f; // Reduce volume
                    isLowPassEnabled = true;
                    isLowPassDisabled = false;
                }
                else if (!enable && !isLowPassDisabled) // Disable low-pass effect if not already disabled
                {
                    audioSource.volume /= 0.2f; // Reset volume
                    isLowPassEnabled = false;
                    isLowPassDisabled = true;
                }
            }
        }
    }

    private void AdjustSwinVolume(bool isMoving)
    {
        if (Swin != null)
        {
            Swin.volume = isMoving ? 0.8f : 0.25f; // Adjust Swin volume based on movement
        }
    }

    private void AdjustSwinLowPass(bool enable)
    {
        if (Swin != null)
        {
            if (enable) // If low-pass is enabled (underwater or paused)
            {
                Swin.volume = 0.1f; // Force volume to 0.25
            }
            else if (!enable) // If low-pass is disabled
            {
                // Restore Swin volume based on movement state
                Swin.volume = (Player.transform.position == lastPlayerPosition) ? 0.1f : 0.7f;
            }
        }
    }

}
