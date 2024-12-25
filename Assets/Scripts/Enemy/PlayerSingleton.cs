using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    private static PlayerSingleton instance;

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
    }
}