using UnityEngine;

public class ParticlePos : MonoBehaviour
{
    public Transform playerTransform; // �������� Transform

    void Update()
    {
        if (playerTransform != null)
        {
            // �õ�ǰ����� Transform ������������ Transform
            transform.position = playerTransform.position;
        }
    }
}
