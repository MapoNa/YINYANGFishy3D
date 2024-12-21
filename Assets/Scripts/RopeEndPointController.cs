using UnityEngine;

public class RopeEndPointController : MonoBehaviour
{
    public Transform startPoint; // 绳子的起始点
    public float maxDistance = 2f; // 最大允许的距离

    void Update()
    {
        if (startPoint == null)
        {
            Debug.LogWarning("Start point is not assigned!");
            return;
        }

        // 计算当前与起始点的距离
        float currentDistance = Vector3.Distance(transform.position, startPoint.position);

        // 如果距离超过最大值，将结束点拉回到最大距离范围内
        if (currentDistance > maxDistance)
        {
            Vector3 direction = (transform.position - startPoint.position).normalized;
            transform.position = startPoint.position + direction * maxDistance;
        }
    }
}
