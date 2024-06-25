using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinWrapper : MonoBehaviour
{
    public int coinType = 1;

    private Vector3 pickedPosition;
    private float pickProgress = 0f;
    private GameObject pickupPlayer;
    private Animator coinAnimator;
    private bool isPicking = false; 
    
    // Start is called before the first frame update
    void Start()
    {
        coinAnimator = GetComponent<Animator>();
        coinAnimator.SetInteger("Coin Type", coinType);
        pickupPlayer = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isPicking)
        {
            pickProgress += Time.deltaTime;
            Vector3 pos;
            pos.x = Mathf.Lerp(pickedPosition.x, pickupPlayer.transform.position.x, pickProgress);
            pos.y = Mathf.Lerp(pickedPosition.y, pickupPlayer.transform.position.y, pickProgress);
            pos.z = 0f;
            transform.position = pos;
            
            if (pickProgress >= 1f)
            {
                isPicking = false;
                AudioSource audioSource = GetComponent<AudioSource>();
                audioSource.Play();
                pickupPlayer.GetComponent<PlayerController>().PickupCoin(coinType);
                Invoke("DestoryAfterAudioComplete", audioSource.clip.length);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPicking)
        {
            return;
        }
        PlayerController playerController = pickupPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            pickedPosition = transform.position;
            Invoke("SetIsPicking", 0.5f);
        }
    }

    void SetIsPicking()
    {
        isPicking = true;
    }

    private void DestoryAfterAudioComplete()
    {
        Destroy(gameObject);
    }
}
