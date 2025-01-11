using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteraction : MonoBehaviour
{
    [SerializeField]
    RenderTexture rt; // Render texture to save the rendered output
    [SerializeField]
    Transform target; // Target player object

    [SerializeField]
    Vector3 offset = new Vector3(0, 10, -10); // Offset between the camera and the player

    void Awake()
    {
        // Set global Shader properties
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
    }

    void Update()
    {
        if (target != null)
        {
            // Track the player's position and maintain the offset
            transform.position = target.position + offset;
        }

        // Update global Shader properties
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize", GetComponent<Camera>().orthographicSize);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
