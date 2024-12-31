using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        CheckPlayerAlive();
    }

    private void CheckPlayerAlive()
    {
        if (PlayerSingleton.Instance.IsAlive) return;
        PlayerSingleton.Instance.transform.position = transform.position;
        PlayerSingleton.Instance.IsAlive = true;
    }
}