using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EatingFishManager : MonoBehaviour
{
    private FishController fishController; // Reference to the FishController component
    private FixedJoint joint; // Reference to the FixedJoint component on the fish mouse transform

    [SerializeField]
    private float eatingSize = 0.5f; // Size at which the fish starts eating

    [SerializeField]
    private LayerMask eatableLayer; // Layer mask for the eatable objects

    [SerializeField]
    private Transform fishMouseTransform; // Transform of the fish mouse

    private Rigidbody redBallRb; // Reference to the red ball's rigidbody
    private Collider redBallCollider; // Reference to the red ball's collider
    private Collider playerCollider; // Reference to the player's collider
    private List<Collider> redBallChildColliders = new List<Collider>(); // List of child colliders of the red ball
    public AudioSource EatingSound; // Reference to the audio source for eating sound

    private void Start()
    {
        // Get the FishController component
        fishController = GetComponent<FishController>();
        // Get the FixedJoint component on the fish mouse transform
        joint = fishMouseTransform.GetComponent<FixedJoint>();

        // Get the player's collider
        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogError("Player collider not found!");
        }
    }

    private void Update()
    {
        // Check if the fish is eating and the object's scale is greater than or equal to the eating size
        if (fishController.isEating && transform.localScale.x >= eatingSize && joint.connectedBody == null)
        {
            // Get all colliders within a small radius around the fish mouse transform
            foreach (var col in GetColliders())
            {
                // Check if the collider has a Rigidbody component
                if (col.GetComponent<Rigidbody>())
                {
                    // Get the Rigidbody component
                    redBallRb = col.GetComponent<Rigidbody>();
                    redBallCollider = col.GetComponent<Collider>(); // Cache the collider

                    // Check if the object's scale is smaller than the red ball's scale
                    if (transform.localScale.x < redBallRb.transform.localScale.x)
                        continue;

                    // Move the red ball to the fish mouse position
                    redBallRb.gameObject.transform.position = fishMouseTransform.position + transform.forward * 0.1f;

                    // Set the red ball as a child of the fish mouse transform
                    redBallRb.transform.parent = fishMouseTransform.transform;

                    // Ignore collision between the player and the red ball, including its children
                    IgnoreCollisionWithChildren(redBallCollider);

                    // Set the red ball to the player
                    SetBallToPlayer(redBallRb);

                    // Log a message with the name of the red ball
                    Debug.Log("Finding a red ball" + redBallRb.gameObject.name);
                    break;
                }
            }
        }

        // Check if the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            // Check if there is a connected body in the fixed joint
            if (joint.connectedBody != null)
            {
                // Remove the ball from the player
                RemoveBallFromPlayer();
            }

            // Get all colliders within a small radius around the fish mouse transform
            foreach (var col in GetColliders())
            {
                // Check if the collider has the IEatable interface
                if (col.GetComponent<IEatable>() != null)
                {
                    // Call the CheckEatable method on the IEatable component with the object's scale
                    col.GetComponent<IEatable>().CheckEatable(transform.localScale);
                    break;
                }
            }
        }
    }

    private void SetBallToPlayer(Rigidbody rb)
    {
        // Play the eating sound
        EatingSound.PlayOneShot(EatingSound.clip);
        // Connect the red ball's rigidbody to the fixed joint
        joint.connectedBody = rb;
    }

    private void RemoveBallFromPlayer()
    {
        // Re-enable collision between the player and the red ball, including its children
        ReEnableCollisionWithChildren();

        // Remove the red ball from the player
        redBallRb.transform.parent = null;
        joint.connectedBody = null;
        redBallRb = null;
        redBallCollider = null;
        redBallChildColliders.Clear();
    }

    private List<Collider> GetColliders()
    {
        // Get all colliders within a small radius around the fish mouse transform
        var colliders = Physics.OverlapSphere(fishMouseTransform.position, 0.1f, eatableLayer);

        return new List<Collider>(colliders);
    }

    private void IgnoreCollisionWithChildren(Collider parentCollider)
    {
        // Ignore collision between the player and the parent collider
        if (parentCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(parentCollider, playerCollider, true);
        }

        // Get all child colliders and ignore collision
        redBallChildColliders = new List<Collider>(parentCollider.GetComponentsInChildren<Collider>());
        foreach (var childCollider in redBallChildColliders)
        {
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(childCollider, playerCollider, true);
            }
        }
    }

    private void ReEnableCollisionWithChildren()
    {
        // Re-enable collision between the player and the parent collider
        if (redBallCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(redBallCollider, playerCollider, false);
        }

        // Re-enable collision between the player and all child colliders
        foreach (var childCollider in redBallChildColliders)
        {
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(childCollider, playerCollider, false);
            }
        }
    }
}
