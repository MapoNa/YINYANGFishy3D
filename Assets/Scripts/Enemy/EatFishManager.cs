using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFishManager : MonoBehaviour
{
    private FishController fishController;
    private bool hasExecuted = false;

    private void Start()
    {
        fishController = GetComponent<FishController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && !hasExecuted)
        {
            hasExecuted = true;
            foreach (var col in fishController.NearByObjects)
            {
                if (col.GetComponent<IEatable>() != null)
                {
                    col.GetComponent<IEatable>().CheckEatable(transform.localScale);
                    break;
                }
            }

            fishController.NearByObjects.Clear();
        }
    }
}