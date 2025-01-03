using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to the OnRespawn event of the PlayerSingleton
        PlayerSingleton.Instance.OnRespawn += RespawnPlayer;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnRespawn event of the PlayerSingleton
        PlayerSingleton.Instance.OnRespawn -= RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        // Respawn the player at the current position
        PlayerSingleton.Instance.transform.position = transform.position;
    }
}