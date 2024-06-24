using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip[] FootStepAudios;

    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayStepAudio()
    {
        audioSource.PlayOneShot(FootStepAudios[Random.Range(0, 3)]);
    }
}
