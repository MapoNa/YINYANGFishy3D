using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // 玩家（鱼）的Transform
    public Vector3 offset = new Vector3(0, 4, -2); // 摄像机偏移量
    public float smoothSpeed = 0.3f; // 平滑速度

    void FixedUpdate()
    {
        if (player != null)
        {
            // 目标位置
            Vector3 desiredPosition = player.position + offset;

            // 平滑跟随位置
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);

            // 平滑旋转到玩家方向
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.fixedDeltaTime);
        }
    }
}
