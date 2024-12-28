using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishAgent : Agent
{
    public Vector2 SizeRange = new Vector2() { x = 0.5f, y = 1.5f };


    private void Start()
    {
        InitFishData();
    }

    private void InitFishData()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            var scaleValue = RandomSize(SizeRange);
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            rb.mass = scaleValue;
        }
    }

    private float RandomSize(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
}