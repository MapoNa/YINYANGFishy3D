using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // ��ң��㣩��Transform
    public Vector3 offset = new Vector3(0, 4, -2); // �����ƫ����
    public float smoothSpeed = 0.3f; // ƽ���ٶ�

    void FixedUpdate()
    {
        if (player != null)
        {
            // Ŀ��λ��
            Vector3 desiredPosition = player.position + offset;

            // ƽ������λ��
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);

            // ƽ����ת����ҷ���
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.fixedDeltaTime);
        }
    }
}
