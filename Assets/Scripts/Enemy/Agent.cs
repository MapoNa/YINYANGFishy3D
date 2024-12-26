using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour, IEatable
{
    [Header("Stats")]
    public float YinYangEffect = 0.1f;
    // Movement speed of the agent

    public float Speed = 1f;

    // Turning speed of the agent
    public float TurnSpeed = 10f;

    // Minimum interval between turns (in seconds)
    public float MinTurnInterval = 2f;

    // Maximum interval between turns (in seconds)
    public float MaxTurnInterval = 6f;

    // Damping factor for smooth rotation 
    public float SmoothDamping = 0.1f;

    protected Rigidbody rb;
    private float turnTimer = 0f;
    private float turnInterval = 0f;
    private Quaternion targetRotation = Quaternion.identity;

    public static event Action<float> OnEaten; // Action 

    private void Start()
    {
        // Get reference to the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Initialize the turn interval to a random value between minTurnInterval and maxTurnInterval
        turnInterval = Random.Range(MinTurnInterval, MaxTurnInterval);
    }

    virtual protected void FixedUpdate()
    {
        // Move the agent
        Move();

        // Turn the agent
        Turn();
    }

    virtual protected void Move()
    {
        // Calculate the movement vector based on the agent's forward direction, speed, and time step
        Vector3 movement = transform.forward * Speed * Time.fixedDeltaTime;

        // Move the agent using Rigidbody.MovePosition
        rb.MovePosition(rb.position + movement);
    }

    virtual protected void Turn()
    {
        // Increment the turn timer by the time step
        turnTimer += Time.fixedDeltaTime;

        // Check if it's time to turn
        if (turnTimer >= turnInterval)
        {
            // Generate a random turning amount and apply it to the agent's rotation
            float turnAmount = Random.Range(-TurnSpeed, TurnSpeed);
            targetRotation = Quaternion.Euler(0f, turnAmount, 0f) * rb.rotation;

            // Reset the turn timer to 0
            turnTimer = 0f;

            // Update the turn interval to a new random value between minTurnInterval and maxTurnInterval
            turnInterval = Random.Range(MinTurnInterval, MaxTurnInterval);
        }

        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * SmoothDamping));
    }

    public void CheckEatable(Vector3 scale)
    {
        if (scale.x > transform.localScale.x)
        {
            //Eaten
            OnEaten?.Invoke(YinYangEffect);

            Destroy(gameObject);
        }
    }
    virtual protected void StartAttack()
    {
       
    }
}