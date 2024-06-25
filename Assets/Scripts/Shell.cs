using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shell : MonoBehaviour
{
    private GameObject playerCharacter;
    private PlayerController playerController;
    private Rigidbody2D rigidbody2d;
    
    public Vector2 dropTimeRange = new Vector2(0.5f, 0.8f);
    public Vector2 dropAngleRange = new Vector2(30.0f, 60.0f);
    public AudioClip[] dropAudios;

    private float dropTime;
    private bool isStopped = false;

    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player");
        playerController = playerCharacter.GetComponent<PlayerController>();
        
        rigidbody2d = GetComponent<Rigidbody2D>();
        Vector2 dir = Quaternion.AngleAxis(
            playerController.GetPlayerFaceDir() *Random.Range(dropAngleRange.x, dropAngleRange.y), Vector3.forward) * Vector2.up
            ;
        rigidbody2d.velocity = dir * 3.0f;
        dropTime = Random.Range(dropTimeRange.x, dropTimeRange.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped)
        {
            return;
        }
        dropTime -= Time.deltaTime;

        transform.rotation = transform.rotation * Quaternion.Euler(0f, 0f, 3.0f);
        
        if (dropTime <= 0)
        {
            isStopped = true;
            rigidbody2d.velocity = Vector2.zero;
            rigidbody2d.gravityScale = 0;
            AudioSource audioSource = GetComponent<AudioSource>();
            AudioClip dropClip = dropAudios[Random.Range(0, dropAudios.Length)];
            audioSource.PlayOneShot(dropClip);
            Invoke("RemoveAudioSource", dropClip.length);
        }
    }

    void RemoveAudioSource()
    {
        AudioSource audioSource = GetComponent<AudioSource>(); 
        Destroy(audioSource);
    }
}
