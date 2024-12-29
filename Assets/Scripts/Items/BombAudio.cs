using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAudio : MonoBehaviour
{
    public AudioSource BombSound;

    private void OnEnable()
    {
        Bomb.OnBombExplode += PlayBombSound;
    }

    private void OnDisable()
    {
        Bomb.OnBombExplode -= PlayBombSound;
    }

    private void PlayBombSound()
    {
        BombSound.PlayOneShot(BombSound.clip);
    }
}