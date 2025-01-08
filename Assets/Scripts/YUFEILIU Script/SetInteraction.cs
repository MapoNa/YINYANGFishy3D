using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteraction : MonoBehaviour
{
    [SerializeField]
    RenderTexture rt; // ���ڱ�����Ⱦ����
    [SerializeField]
    Transform target; // �������

    [SerializeField]
    Vector3 offset = new Vector3(0, 10, -10); // ���������ҵ�ƫ����

    void Awake()
    {
        // ����ȫ�� Shader ����
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
    }

    void Update()
    {
        if (target != null)
        {
            // ׷�����λ�ò�����ƫ��
            transform.position = target.position + offset;
        }

        // ���� Shader ȫ������
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
