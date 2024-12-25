using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossAgent : Agent
{
    public float ChaseSpeed = 15f;
    public float Acceleration = 0.5f;

    public int Hp = 3;
    public int ShieldPoint = 1;
    public float AttackDelayTimer = 2f;

    public float DetectPlayerDistance = 15f;
    public Vector2 TimeBetweenAttacks;
    private Animator animatorAI;
    private float randomTimer;
    private DistanceChecker distanceChecker;

    private void Start()
    {
        animatorAI = GetComponent<Animator>();
        distanceChecker = GetComponent<DistanceChecker>();
    }

    private float RandomTimer()
    {
        var randomValue = Random.Range(TimeBetweenAttacks.x, TimeBetweenAttacks.y);

        return randomValue;
    }

    // public void SelectSkills()
    // {
    //     if (randomTimer > 0)
    //     {
    //         randomTimer -= Time.deltaTime;
    //     }
    //     else
    //     {
    //         SetAttack();
    //         randomTimer = RandomTimer();
    //     }
    // }

    // IEnumerator SetAttack()
    // {
    //     yield return new WaitForSeconds(AttackDelayTimer);
    //     animatorAI.SetTrigger(_attackState);
    // }

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
    }

    public void ChaseMove()
    {
        transform.LookAt(PlayerSingleton.Instance.transform);
        var dir = distanceChecker.DirectionFromPlayer();

        var desiredVelocity = Vector3.zero;
        desiredVelocity = dir * ChaseSpeed;

        rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, Acceleration);
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