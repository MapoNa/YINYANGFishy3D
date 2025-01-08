using UnityEngine;

public class RopeEndPointController : MonoBehaviour
{
    public Transform startPoint; // ���ӵ���ʼ��
    public float maxDistance = 2f; // �������ľ���

    void Update()
    {
        if (startPoint == null)
        {
            Debug.LogWarning("Start point is not assigned!");
            return;
        }

        // ���㵱ǰ����ʼ��ľ���
        float currentDistance = Vector3.Distance(transform.position, startPoint.position);

        // ������볬�����ֵ�������������ص������뷶Χ��
        if (currentDistance > maxDistance)
        {
            Vector3 direction = (transform.position - startPoint.position).normalized;
            transform.position = startPoint.position + direction * maxDistance;
        }
    }
}
