using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAudio : MonoBehaviour
{
    public AudioSource BombSound; // Reference to the AudioSource component for the bomb sound

    private void OnEnable()
    {
        // Subscribe to the OnBombExplode event of the Bomb class
        Bomb.OnBombExplode += PlayBombSound;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnBombExplode event of the Bomb class
        Bomb.OnBombExplode -= PlayBombSound;
    }

    private void PlayBombSound()
    {
        // Play the bomb sound using the AudioSource component
        BombSound.PlayOneShot(BombSound.clip);
    }
}