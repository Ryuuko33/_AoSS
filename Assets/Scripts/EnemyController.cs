using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health = 30;
    public int Speed = 3;
    public float FilpTime = 0.05f;
    public float dyingTime = 0.1f;
    public Material hurtMaterial;
    public EnemyType enemyType = EnemyType.Normal;
    public enum EnemyType
    {
        Normal,
        Exporsion,
        KeepLaunch,
        DeathLaunch
    }

    private GameObject playerCharacter;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;

    private Material defaultMaterial;
    private Vector2 move;
    private Vector2 deadPos;
    private bool isDying = false;
    private bool isFlipping = false;
    private bool isDead;
    private int faceDir = -1;
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        playerCharacter = GameObject.FindWithTag("Player");
        
        defaultMaterial = spriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if (isDying)
            {
                float delta =  Time.deltaTime / dyingTime;
                float NewscaleY = transform.localScale.y + delta;
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(NewscaleY, -1, 1), 1);

                transform.position = new Vector2(deadPos.x, Mathf.Lerp(deadPos.y, deadPos.y + 1f, Mathf.Sin(Mathf.PI * NewscaleY)));
                if (Mathf.Abs(NewscaleY) >= 1.0f)
                {
                    isDying = false;
                }
            }
            return;
        }
        Vector3 moveDirection = playerCharacter.transform.position - transform.position;
        move = moveDirection.normalized;
        
        float angle = Vector2.Angle(Vector2.right, moveDirection);
        if ((faceDir < 0 && angle > 90.0f) || (faceDir > 0 && angle <= 90.0f))
        { 
            faceDir = -faceDir;
            isFlipping = true;
        }

        // 翻转人物
        if (isFlipping)
        {
            float deltaScale =  2 * Time.deltaTime / FilpTime;
            float NewscaleX = transform.localScale.x + faceDir * deltaScale;
            transform.localScale = new Vector3(Mathf.Clamp(NewscaleX, -1, 1), 1, 1);
            if (Mathf.Abs(NewscaleX) >= 1.0f)
            {
                isFlipping = !isFlipping;
            }
        }
    }

    public void SetFaceDir(int faceDir)
    {
        this.faceDir = faceDir;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }
        Vector2 position = (Vector2)transform.position + move * ((float)Speed * Time.deltaTime);
        rigidbody2d.MovePosition(position);
    }

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            SetSelfDead();
        }

        spriteRenderer.material = hurtMaterial;
        Invoke("SetDefaultMaterial", 0.05f);
        System.Threading.Thread.Sleep(5);

    }

    void SetSelfDead()
    {
        if (!isDead)
        {
            GetComponent<Animator>().SetBool("IsDead", true);
            transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.localScale = new Vector3(transform.localScale.x, 0, 1);
            isDead = true;
            isDying = true;
            deadPos = transform.position;
            spriteRenderer.color = Color.grey;
            spriteRenderer.sortingLayerName = "Default";
        }
    }

    void SetDefaultMaterial()
    {
        spriteRenderer.material = defaultMaterial;
    }
}
