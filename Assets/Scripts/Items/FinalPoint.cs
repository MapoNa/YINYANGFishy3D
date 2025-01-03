using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPoint : MonoBehaviour
{
    public GameObject FinalPointUI;
    public AudioSource FinalPointSound;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is a FishController
        if (other.GetComponent<FishController>())
        {
            // Activate the FinalPointUI and play the FinalPointSound
            FinalPointUI.SetActive(true);
            // Play the FinalPointSound
            FinalPointSound.PlayOneShot(FinalPointSound.clip);
        }
    }
}