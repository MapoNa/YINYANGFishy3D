using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestorySelf : MonoBehaviour
{
    public float destroyTime = 3f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}