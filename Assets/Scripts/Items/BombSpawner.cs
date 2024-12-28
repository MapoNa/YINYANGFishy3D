using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab;

    [SerializeField]
    private float spawnInterval = 5f;

    private bool canSpawn = true;

    public Material StopSpawningMaterial;

    public Transform SpawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<FishController>() && canSpawn)
        {
            Instantiate(bombPrefab, SpawnPoint.position, Quaternion.identity);
            canSpawn = false;
            StopSpawningMaterial.DOColor(Color.red, 1f);
            StartCoroutine(SpanwTimer());
        }
    }

    IEnumerator SpanwTimer()
    {
        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
        StopSpawningMaterial.DOColor(Color.green, 1f);
    }
}