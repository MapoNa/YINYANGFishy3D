 using UnityEngine;

public class ShadowDetection : MonoBehaviour
{
    public Light mainLight; // 场景中的主光源
    public LayerMask shadowCastingLayers; // 用于检测阴影的图层
    public bool isInShadow = false; // 当前是否在阴影中

    void Start()
    {
        if (mainLight == null)
        {
            // 自动查找主光源
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
        // 获取光源方向
        Vector3 lightDirection = -mainLight.transform.forward;

        // 从物体位置沿光源方向投射射线
        Ray ray = new Ray(transform.position, lightDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, shadowCastingLayers))
        {
            // 如果射线命中其他物体，说明在阴影中
            isInShadow = true;
        }
        else
        {
            // 没有遮挡物，则不在阴影中
            isInShadow = false;
        }

        Debug.Log($"Is in Shadow: {isInShadow}");
    }
}
