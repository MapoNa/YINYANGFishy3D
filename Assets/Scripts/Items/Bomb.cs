using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float explosionRadius = 5f; // Radius of the explosion

    [SerializeField]
    private float explosionForce = 10f; // Force applied to objects in the explosion

    [SerializeField]
    private float timeToExplode = 3f; // Time before the bomb explodes

    [SerializeField]
    private LayerMask explosionLayer; // Layer mask for the objects that can be affected by the explosion

    private AudioSource beep; // Reference to the AudioSource component
    public GameObject explodePrefab; // Prefab for the explosion effect
    public Transform ExplodeTransform; // Transform of the explosion effect
    public static event Action OnBombExplode; // Event that is triggered when the bomb explodes

    private void Start()
    {
        Invoke("Explode", timeToExplode); // Schedule the explosion to occur after the specified time
        beep = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    public void Explode()
    {
        Collider[] colliders = new Collider[50]; // Create an array to store the colliders
        int numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, explosionRadius,
                                          colliders, explosionLayer); // Get the colliders and store them in the array
        for (int i = 0; i < numColliders; i++)
        {
            if (colliders[i].GetComponent<Rigidbody>())
            {
                colliders[i].GetComponent<Rigidbody>()
                            .AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f,
                                               ForceMode.Impulse); // Apply explosion force to the rigidbody
            }

            if (colliders[i].GetComponent<Wall>())
            {
                Destroy(colliders[i].gameObject); // Destroy the wall object
            }

            if (colliders[i].GetComponent<BossAgent>())
            {
                var bossAgent = colliders[i].GetComponent<BossAgent>();
                bossAgent.TakeDamage(1); // Apply damage to the boss agent
            }

            if (colliders[i].GetComponent<PlayerSingleton>())
            {
                var player = colliders[i].GetComponent<PlayerSingleton>();
                player.IsAlive = false; // Set the player's IsAlive flag to false
                PlayerSingleton.Instance.CheckPlayerAlive();
            }
        }

        Instantiate(explodePrefab, ExplodeTransform.transform.position,
                    Quaternion.identity); // Instantiate the explosion effect
        OnBombExplode?.Invoke(); // Trigger the OnBombExplode event
        Destroy(transform.gameObject); // Destroy the bomb object
    }

    public void PlayBeep()
    {
        beep.PlayOneShot(beep.clip); // Play the beep sound
    }
}