using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFishSpawnManager : MonoBehaviour
{
    public GameObject FishPrefab; // Prefab for the fish object

    public Transform SpawnPoint; // Transform of the spawn point

    public int FishCount = 10; // Number of fish to spawn
    public float SpawnInterval = 5f; // Interval between fish spawns
    public List<Agent> FishList = new List<Agent>(); // List of spawned fish
    private float spawnTimer = 2f; // Timer for spawning fish

    private void OnEnable()
    {
        // Subscribe to the OnEaten event of the Agent class
        Agent.OnEaten += OnEaten;
    }

    private void Update()
    {
        // Increment the spawn timer by the time elapsed since the last frame
        spawnTimer += Time.deltaTime;
        // Check if the number of fish is less than the desired count and the spawn timer has reached the interval
        if (FishList.Count < FishCount && spawnTimer > SpawnInterval)
        {
            // Spawn a fish
            SpawnFish();
            // Reset the spawn timer
            spawnTimer = 0f;
        }
    }

    private void SpawnFish()
    {
        // Instantiate the fish object at the spawn point position
        var fish = Instantiate(FishPrefab, SpawnPoint.position, Quaternion.identity);
        // Get the Agent component of the fish object and add it to the list of spawned fish
        FishList.Add(fish.GetComponent<Agent>());
    }

    private void OnEaten(Agent agentFish)
    {
        // Remove the eaten fish from the list of spawned fish
        FishList.Remove(agentFish);
    }
}