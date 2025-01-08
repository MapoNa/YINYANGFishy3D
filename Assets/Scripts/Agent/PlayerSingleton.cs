using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    private static PlayerSingleton instance; // Static instance of the PlayerSingleton class
    public bool IsAlive = true; // Flag to check if the player is alive
    public event Action OnRespawn; // Event that is triggered when the player respawns
    private FishController fishController; // Reference to the FishController component
    public float RespawnTime = 3f; // Time it takes for the player to respawn
    public GameObject DeadUI; // Reference to the UI object that is displayed when the player dies

    public static PlayerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                // Check if an instance of the PlayerSingleton class already exists
                instance = FindObjectOfType<PlayerSingleton>();

                if (instance == null)
                {
                    // If no instance exists, create a new game object and add the PlayerSingleton component to it
                    GameObject playerObject = new GameObject("Player");
                    instance = playerObject.AddComponent<PlayerSingleton>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        // If no instance exists or this is the first instance, set the instance to this object
        instance = this;
    }

    public void CheckPlayerAlive()
    {
        if (IsAlive) return; // If the player is already alive, do nothing
        fishController = GetComponent<FishController>(); // Get the FishController component
        StartCoroutine(RespawnPlayer()); // Start the coroutine to respawn the player
    }

    private IEnumerator RespawnPlayer()
    {
        DeadUI.SetActive(true); // Activate the UI object that is displayed when the player dies
        fishController.enabled = false; // Disable the FishController component
        yield return new WaitForSeconds(RespawnTime); // Wait for the specified respawn time
        OnRespawn?.Invoke(); // Trigger the OnRespawn event
        IsAlive = true; // Set the IsAlive flag to true
        fishController.enabled = true; // Enable the FishController component
        DeadUI.SetActive(false); // Deactivate the UI object
    }
}