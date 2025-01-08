using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossAgent : Agent
{
    // Static readonly variables for hash values
    static private readonly int attackState = Animator.StringToHash("Attack");
    static private readonly int _speed = Animator.StringToHash("Speed");

    // Public variables for agent properties
    public float ChaseSpeed = 15f; // Speed at which the agent chases the player
    public float Acceleration = 0.5f; // Acceleration of the agent
    public int Hp = 3; // Health points of the agent
    public int ShieldPoint = 1; // Number of shield points the agent has
    public float AttackDelayTimer = 2f; // Delay before the agent can attack again

    public float DetectPlayerDistance = 15f; // Distance at which the agent detects the player
    public Vector2 TimeBetweenAttacks; // Range of time between attacks
    private Animator animatorAI; // Animator component of the agent
    public float randomTimer = 5f; // Random timer for selecting skills
    private DistanceChecker distanceChecker; // Distance checker component of the agent
    private bool canMoving = true; // Whether the agent can move

    public Collider AttackCollider; // Collider used for attacking the player

    public Image[] hpImages; // Images used to display the agent's health points
    public GameObject ShieldMaterial; // Material used for the agent's shield
    public GameObject ShieldImage; // Image used to display the agent's shield

    public GameObject KeyPrefab; // Prefab for the key object
    public Wall WallPrefab; // Prefab for the wall object

    private void Start()
    {
        // Initialize the animator and distance checker components
        animatorAI = GetComponent<Animator>();
        distanceChecker = GetComponent<DistanceChecker>();
    }

    private float RandomTimer()
    {
        // Generate a random timer value within the range defined by TimeBetweenAttacks
        var randomValue = Random.Range(TimeBetweenAttacks.x, TimeBetweenAttacks.y);

        return randomValue;
    }

    public void SelectSkills()
    {
        // Select skills for the agent based on the randomTimer
        if (randomTimer > 0)
        {
            randomTimer -= Time.deltaTime;
        }
        else
        {
            SetAttack();
            randomTimer = RandomTimer();
        }
    }

    private void SetAttack()
    {
        // Set the agent to attack the player
        canMoving = false;
        animatorAI.SetTrigger(attackState);
    }

    override protected void StartAttack()
    {
        // Start the attack animation
        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        // Perform the attack animation by moving towards the player
        AttackCollider.enabled = true;
        var endPos = PlayerSingleton.Instance.transform.position;
        transform.LookAt(PlayerSingleton.Instance.transform);
        rb.DOMove(endPos, AttackDelayTimer + 1f);

        yield return new WaitForSeconds(AttackDelayTimer);
        canMoving = true;
        AttackCollider.enabled = false;
    }

    public void TakeDamage(int damage)
    {
        // Take damage from the player and update the agent's health points
        if (ShieldPoint > 0) return;
        var hpImage = hpImages[Mathf.Abs(Hp - 3)]
           .GetComponent<Image>();
        Hp = Mathf.Max(0, Hp - damage);
        hpImage.DOColor(Color.gray, 0.5f);
        if (Hp <= 0)
        {
            // Instantiate the key object and destroy the agent's game object
            Instantiate(KeyPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    override protected void FixedUpdate()
    {
        // Check if the player is detected and the agent can move
        if (DetectedPlayer() && canMoving)
        {
            // Select skills for the agent
            SelectSkills();
            // Perform chase movement
            ChaseMove();
        }
        // If can move but not detected, perform base movement
        else if (canMoving)
        {
            BaseMove();
        }
        // If neither detected nor can move, set velocity to zero
        else
        {
            rb.velocity = Vector3.zero;
        }

        // Update the agent's speed parameter in the animator
        animatorAI.SetFloat(_speed, rb.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public void ChaseMove()
    {
        // Set the agent's forward direction to face the player
        transform.LookAt(PlayerSingleton.Instance.transform);
        // Calculate the desired velocity based on the chase speed
        var desiredVelocity = transform.forward * ChaseSpeed;
        // Smoothly interpolate the agent's velocity towards the desired velocity using acceleration
        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, Acceleration);
    }

    public void BaseMove()
    {
        // Call the Move() method to move the agent
        Move();
        // Call the Turn() method to turn the agent towards the target direction
        Turn();
    }

    public bool DetectedPlayer()
    {
        // Check if the wall prefab exists
        if (WallPrefab) return false;
        // Check if the distance from the player is less than the detect player distance
        if (distanceChecker.DistanceFromPlayer() < DetectPlayerDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the Pillar component
        if (other.GetComponent<Pillar>())
        {
            // Decrease the shield point, disable the shield material and image, and disable the collided object
            ShieldPoint--;
            ShieldMaterial.gameObject.SetActive(false);
            ShieldImage.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            // Kill the agent's rigidbody dotween animation
            DOTween.Kill(rb);
        }

        // Check if the collided object has the FishController component
        if (other.GetComponent<FishController>())
        {
            // Set the player's IsAlive property to false and call the CheckPlayerAlive() method
            PlayerSingleton.Instance.IsAlive = false;
            PlayerSingleton.Instance.CheckPlayerAlive();
        }
    }
}