using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossAgent : Agent
{
    static private readonly int attackState = Animator.StringToHash("Attack");
    static private readonly int _speed = Animator.StringToHash("Speed");

    public float ChaseSpeed = 15f;
    public float Acceleration = 0.5f;
    public float SmashSpeed = 100f;
    public int Hp = 3;
    public int ShieldPoint = 1;
    public float AttackDelayTimer = 2f;

    public float DetectPlayerDistance = 15f;
    public Vector2 TimeBetweenAttacks;
    private Animator animatorAI;
    public float randomTimer = 5f;
    private DistanceChecker distanceChecker;
    private bool canMoving = true;

    private void Start()
    {
        animatorAI = GetComponent<Animator>();
        distanceChecker = GetComponent<DistanceChecker>();
        rb = GetComponent<Rigidbody>();
    }

    private float RandomTimer()
    {
        var randomValue = Random.Range(TimeBetweenAttacks.x, TimeBetweenAttacks.y);

        return randomValue;
    }

    public void SelectSkills()
    {
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
        canMoving = false;
        animatorAI.SetTrigger(attackState);
    }

    override protected void StartAttack()
    {
        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        var endPos = PlayerSingleton.Instance.transform.position;
        rb.DOMove(endPos, AttackDelayTimer);
        yield return new WaitForSeconds(AttackDelayTimer);
        canMoving = true;
    }


    public void TakeDamage(int damage)
    {
        Hp = Mathf.Max(0, Hp - damage);
        if (Hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    override protected void FixedUpdate()
    {
        if (DetectedPlayer() && canMoving)
        {
            SelectSkills();
            ChaseMove();
        }
        else if (canMoving)
        {
            BaseMove();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        animatorAI.SetFloat(_speed, rb.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public void ChaseMove()
    {
        Debug.Log("chase move");
        transform.LookAt(PlayerSingleton.Instance.transform);

        var desiredVelocity = transform.forward * ChaseSpeed;
        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, Acceleration);
        Debug.Log(rb.velocity);
    }

    public void BaseMove()
    {
        Move();
        Turn();
    }


    public bool DetectedPlayer()
    {
        if (distanceChecker.DistanceFromPlayer() < DetectPlayerDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}