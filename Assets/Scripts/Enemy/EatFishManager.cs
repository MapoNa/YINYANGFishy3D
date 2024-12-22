using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFishManager : MonoBehaviour
{
    private FishController fishController;

    private void Start()
    {
        fishController = GetComponent<FishController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < fishController.NearByObjects.Length; i++)
            {
                if (fishController.NearByObjects[i] != null)
                {
                    if (fishController.NearByObjects[i].TryGetComponent(out IEatable eatable))
                    {
                        eatable.CheckEatable(fishController.transform.localScale);
                        fishController.NearByObjects[i] = null;
                        break;
                    }
                }
            }
        }
    }
}