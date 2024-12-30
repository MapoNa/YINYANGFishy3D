using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFishSpawnManager : MonoBehaviour
{
    public GameObject FishPrefab;

    public Transform SpawnPoint;

    public int FishCount = 10;
    public float SpawnInterval = 5f;
    public List<Agent> FishList = new List<Agent>();
    private float spawnTimer = 0f;

    private void OnEnable()
    {
        Agent.OnEaten += OnEaten;
    }

    private void Start()
    {
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (FishList.Count < FishCount && spawnTimer > SpawnInterval)
        {
            SpawnFish();
            spawnTimer = 0f;
        }
    }

    private void SpawnFish()
    {
        var fish = Instantiate(FishPrefab, SpawnPoint.position, Quaternion.identity);
        FishList.Add(fish.GetComponent<Agent>());
    }

    private void OnEaten(Agent agentFish)
    {
        FishList.Remove(agentFish);
    }
}