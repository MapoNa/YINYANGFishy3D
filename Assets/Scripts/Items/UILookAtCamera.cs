using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{

    private Camera _mainCamera;

    private void Start()
    {
        // Get the main camera
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // Look at the camera
        if (gameObject.activeSelf)
            gameObject.transform
                      .LookAt(transform.position + _mainCamera.gameObject.transform.rotation * Vector3.forward,
                              _mainCamera.transform.rotation * Vector3.up);
    }
}