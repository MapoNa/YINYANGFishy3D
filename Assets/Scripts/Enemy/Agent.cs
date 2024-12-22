using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // Movement speed of the agent
    public float speed = 1f;

    // Turning speed of the agent
    public float turnSpeed = 10f;

    // Minimum interval between turns (in seconds)
    public float minTurnInterval = 2f;

    // Maximum interval between turns (in seconds)
    public float maxTurnInterval = 6f;

    // Damping factor for smooth rotation 
    public float smoothDamping = 0.1f;

    private Rigidbody rb;
    private float turnTimer = 0f;
    private float turnInterval = 0f;
    private Quaternion targetRotation = Quaternion.identity;

    private void Start()
    {
        // Get reference to the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Initialize the turn interval to a random value between minTurnInterval and maxTurnInterval
        turnInterval = Random.Range(minTurnInterval, maxTurnInterval);
    }

    private void FixedUpdate()
    {
        // Move the agent
        Move();

        // Turn the agent
        Turn();
    }

    private void Move()
    {
        // Calculate the movement vector based on the agent's forward direction, speed, and time step
        Vector3 movement = transform.forward * speed * Time.fixedDeltaTime;

        // Move the agent using Rigidbody.MovePosition
        rb.MovePosition(rb.position + movement);
    }

    private void Turn()
    {
        // Increment the turn timer by the time step
        turnTimer += Time.fixedDeltaTime;

        // Check if it's time to turn
        if (turnTimer >= turnInterval)
        {
            // Generate a random turning amount and apply it to the agent's rotation
            float turnAmount = Random.Range(-turnSpeed, turnSpeed);
            targetRotation = Quaternion.Euler(0f, turnAmount, 0f) * rb.rotation;

            // Reset the turn timer to 0
            turnTimer = 0f;

            // Update the turn interval to a new random value between minTurnInterval and maxTurnInterval
            turnInterval = Random.Range(minTurnInterval, maxTurnInterval);
        }

        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * smoothDamping));
    }
}