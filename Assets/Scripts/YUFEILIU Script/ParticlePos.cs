using UnityEngine;

public class ParticlePos : MonoBehaviour
{
    public Transform playerTransform; // 玩家物体的 Transform

    void Update()
    {
        if (playerTransform != null)
        {
            // 让当前物体的 Transform 等于玩家物体的 Transform
            transform.position = playerTransform.position;
        }
    }
}
