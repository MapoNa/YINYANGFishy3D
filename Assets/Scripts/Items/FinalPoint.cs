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
        if (other.GetComponent<FishController>())
        {
            FinalPointUI.SetActive(true);
            FinalPointSound.PlayOneShot(FinalPointSound.clip);
        }
    }
}