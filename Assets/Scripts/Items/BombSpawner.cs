using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab; // Prefab for the bomb object

    [SerializeField]
    private float spawnInterval = 5f; // Interval between bomb spawns

    private bool canSpawn = true; // Flag to check if bombs can be spawned

    public Material StopSpawningMaterial; // Material to indicate when bomb spawning is stopped

    public Transform Pannel; // Transform of the panel where bombs are spawned

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<FishController>() && canSpawn)
        {
            // Instantiate the bomb object at the panel's position
            var bomb = Instantiate(bombPrefab, Pannel.transform.position, Quaternion.identity);
            canSpawn = false; // Set the canSpawn flag to false to prevent further bomb spawns
            StopSpawningMaterial
               .DOColor(Color.red, 0.5f); // Change the material color to red to indicate bomb spawning is stopped
            StartCoroutine(SpanwTimer()); // Start the coroutine to reset the canSpawn flag and material color after the spawn interval
        }
    }

    IEnumerator SpanwTimer()
    {
        yield return new WaitForSeconds(spawnInterval); // Wait for the specified spawn interval
        canSpawn = true; // Set the canSpawn flag to true to allow bomb spawns again
        StopSpawningMaterial.DOColor(Color.green,
                                     0.5f); // Change the material color to green to indicate bomb spawning is allowed
    }
}