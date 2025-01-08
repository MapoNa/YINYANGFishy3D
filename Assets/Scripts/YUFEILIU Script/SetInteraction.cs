using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteraction : MonoBehaviour
{
    [SerializeField]
    RenderTexture rt; // 用于保存渲染纹理
    [SerializeField]
    Transform target; // 玩家物体

    [SerializeField]
    Vector3 offset = new Vector3(0, 10, -10); // 摄像机与玩家的偏移量

    void Awake()
    {
        // 设置全局 Shader 属性
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
    }

    void Update()
    {
        if (target != null)
        {
            // 追踪玩家位置并保持偏移
            transform.position = target.position + offset;
        }

        // 更新 Shader 全局属性
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
