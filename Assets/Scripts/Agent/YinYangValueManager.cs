using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YinYangValueManager : MonoBehaviour
{
    private FishController fishController; // Reference to the FishController component

    private void OnEnable()
    {
        // Subscribe to the OnEaten event of the Agent class
        Agent.OnEaten += OnEaten;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnEaten event of the Agent class
        Agent.OnEaten -= OnEaten;
    }

    private void Start()
    {
        // Get the FishController component
        fishController = GetComponent<FishController>();
    }

    private void OnEaten(Agent agentFish)
    {
        // Increase the yinYangValue of the FishController by the YinYangEffect of the eaten fish
        fishController.yinYangValue += agentFish.YinYangEffect;
    }
}