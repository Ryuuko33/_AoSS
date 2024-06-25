using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D;
    private CameraShake cameraShake;
    
    public float maxDistance = 100.0f;
    public float force = 300.0f;
    public int damage = 3;
    public GameObject hitVFXPrefab;
    public float shakeStrength = 3.0f;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetInverseBulletSprite(bool isInverse)
    {
        if (!isInverse)
        {
            return;
        }

        transform.localScale = new Vector3(-1, 1, 1);
    }
    
    public void Launch(Vector2 direction)
    {
        rigidbody2D.AddForce(direction * force);
    }

    public float GetForce()
    {
        return force; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            Instantiate(hitVFXPrefab, other.contacts[0].point, Quaternion.FromToRotation(Vector3.right, other.contacts[0].normal));
            enemy.TakeDamage(damage);
            cameraShake.Shake(shakeStrength);
            
            Destroy(gameObject);
        }
    }
}
