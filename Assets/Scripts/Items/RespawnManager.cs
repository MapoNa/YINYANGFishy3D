using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        // Subscribe to the OnRespawn event of the PlayerSingleton
        PlayerSingleton.OnRespawn += RespawnPlayer;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnRespawn event of the PlayerSingleton
        PlayerSingleton.OnRespawn -= RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        // Respawn the player at the current position
        PlayerSingleton.Instance.transform.position = transform.position;
        PlayerSingleton.Instance.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }
}