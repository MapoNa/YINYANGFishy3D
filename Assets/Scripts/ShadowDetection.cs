 using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Light mainLight; // �����е�����Դ
    public LayerMask shadowCastingLayers; // ���ڼ����Ӱ��ͼ��
    public bool isInShadow = false; // ��ǰ�Ƿ�����Ӱ��

    void Start()
    {
        if (mainLight == null)
        {
            // �Զ���������Դ
            mainLight = FindObjectOfType<Light>();
            if (mainLight == null)
            {
                Debug.LogError("No Light found in the scene!");
                return;
            }
        }
    }

    void Update()
    {
        DetectShadow();
    }

    void DetectShadow()
    {
        // ��ȡ��Դ����
        Vector3 lightDirection = -mainLight.transform.forward;

        // ������λ���ع�Դ����Ͷ������
        Ray ray = new Ray(transform.position, lightDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, shadowCastingLayers))
        {
            // ������������������壬˵������Ӱ��
            isInShadow = true;
        }
        else
        {
            // û���ڵ��������Ӱ��
            isInShadow = false;
        }

        Debug.Log($"Is in Shadow: {isInShadow}");
    }
}
