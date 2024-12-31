using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerSingleton.Instance.OnRespawn += RespawnPlayer;
    }

    private void OnDisable()
    {
        PlayerSingleton.Instance.OnRespawn -= RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        PlayerSingleton.Instance.transform.position = transform.position;
    }
}