using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YinYangValueManager : MonoBehaviour
{
    private FishController fishController;

    private void OnEnable()
    {
        Agent.OnEaten += OnEaten;
    }

    private void OnDisable()
    {
        Agent.OnEaten -= OnEaten;
    }

    private void Start()
    {
        fishController = GetComponent<FishController>();
    }

    private void OnEaten(Agent agentFish)
    {
        fishController.yinYangValue += agentFish.YinYangEffect;
    }
}