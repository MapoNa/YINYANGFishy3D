using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    private static PlayerSingleton instance;
    public bool IsAlive = true;
    public event Action OnRespawn;
    private FishController fishController;
    public float RespawnTime = 3f;
    public GameObject DeadUI;

    public static PlayerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerSingleton>();

                if (instance == null)
                {
                    GameObject playerObject = new GameObject("Player");
                    instance = playerObject.AddComponent<PlayerSingleton>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        fishController = GetComponent<FishController>();
    }

    public void CheckPlayerAlive()
    {
        if (IsAlive) return;
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        DeadUI.SetActive(true);
        fishController.enabled = false;
        yield return new WaitForSeconds(RespawnTime);
        OnRespawn?.Invoke();
        IsAlive = true;
        fishController.enabled = true;
        DeadUI.SetActive(false);
    }
}