using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public InputAction FireAction;
    public InputAction MoveAction;
    public InputAction SwitchAction;
    public Material hurtMaterial;
    
    public int health = 30;
    public int speed = 3;
    public float FilpTime = 0.05f;
    public float invisableTime = 0.5f;
    public int[] levelupNeedCoin;
    
    private GameObject activeWeapon;
    private WeaponController weaponController;
    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    private Transform weaponSlot;
    private Canvas playerCanvas;
    private TextMeshProUGUI coinNumTextGUI;
    private TextMeshProUGUI levelTextGUI;
    private Slider sliderExpProgress;
    private WeaponUI weaponUI;

    private int level = 0;
    private int coinNum = 0;
    private int activeWeaponIndex = 0;
    private Material defaultMaterial;
    private Vector2 move;
    private GameObject[] guns;
    private bool isFiring = false;
    private bool isFlipping = false;
    private bool isInvisable = false;
    private float invisableCD;
    private bool isDead = false;
    private int faceDir = 1;

    void Start()
    {
        FireAction.Enable();
        MoveAction.Enable();
        SwitchAction.Enable();

        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCanvas = GameObject.FindFirstObjectByType<Canvas>();
        weaponSlot = transform.Find("WeaponSlot");
        coinNumTextGUI = playerCanvas.transform.Find("Text_CoinNum").GetComponent<TextMeshProUGUI>();
        levelTextGUI = playerCanvas.transform.Find("Text_Level").GetComponent<TextMeshProUGUI>();
        sliderExpProgress = playerCanvas.transform.Find("Slider_Exp").GetComponent<Slider>();
        weaponUI = playerCanvas.transform.Find("WeaponPanel").GetComponent<WeaponUI>();
        
        defaultMaterial = spriteRenderer.material;

        FireAction.performed += BeginFire;
        FireAction.canceled  += StopFire;
        SwitchAction.performed += SwitchWeapon;

        Levelup();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        // 人物移动
        move = MoveAction.ReadValue<Vector2>();
        animator.SetBool("IsMoving", move.magnitude > 0);
        animator.SetFloat("Move X", move.x);
        
        // 获得人物面向
        Vector3 mousePos = Input.mousePosition;
        Vector3 objPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = mousePos - objPos;
        float angle = Vector2.Angle(Vector2.right, direction);
        if ((faceDir > 0 && angle > 90.0f) || (faceDir < 0 && angle <= 90.0f))
        { 
            faceDir = -faceDir;
            isFlipping = true;
        }
        animator.SetFloat("Face Dir", faceDir);
        
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

        if (isInvisable)
        {
            invisableCD -= Time.deltaTime;
            if (invisableCD <= 0)
            {
                SetIsInivable(false);
            }
            spriteRenderer.color = Color.Lerp(Color.white, new Color(1,1,1,0.5f), Mathf.Sin(invisableCD * 10f));
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = (Vector2)transform.position + move * ((float)speed * Time.deltaTime);
        rigidbody2d.MovePosition(position);

    }

    void BeginFire(InputAction.CallbackContext context)
    {
        isFiring = true;
        if (weaponController != null)
        {
            weaponController.SetFiring(isFiring);
        }
        speed -= 2;
    }

    void StopFire(InputAction.CallbackContext context)
    {
        isFiring = false;

        if (weaponController != null)
        {
            weaponController.SetFiring(isFiring);
        }

        speed += 2;
    }

    void SwitchWeapon(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        int index = activeWeaponIndex + (int)value;
        if (index < 0)
        {
            index = weaponSlot.childCount - 1;
        }
        else if (index >= level)
        {
            index = 0;
        }
        SetActiveWeapon(index);
    }

    void SetActiveWeapon(int index)
    {
        GameObject lastWeaponObject = weaponSlot.GetChild(activeWeaponIndex).gameObject;
        lastWeaponObject.GetComponent<WeaponController>().SetFiring(false);
        lastWeaponObject.SetActive(false);
        activeWeaponIndex = Math.Clamp(index, 0, level - 1);
        GameObject weaponObject = weaponSlot.GetChild(activeWeaponIndex).gameObject;
        weaponObject.SetActive(true);
        weaponController = weaponObject.GetComponent<WeaponController>();
        weaponUI.SetActiveWeaponImage(activeWeaponIndex);
    }

    public int GetPlayerFaceDir()
    {
        return this.faceDir;
    }

    public void AddRecoil(Vector2 direction, float Value)
    {
        Value = Mathf.Min(300f, Value);
        rigidbody2d.AddForce(direction * Value);
    }

    public void TakeDamage(int value)
    {
        if (isInvisable)
        {
         return;   
        }
        health -= value;
        if (health <= 0)
        {
            SetSelfDead();
        }

        SetIsInivable(true);
        spriteRenderer.material = hurtMaterial;
        Invoke("SetDefaultMaterial", 0.1f);
    }

    void SetSelfDead()
    {
        if (!isDead)
        {
            isDead = true;
            transform.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void SetIsInivable(bool isInv)
    {
        isInvisable = isInv;
        transform.GetComponent<BoxCollider2D>().enabled = !isInv;
        if (isInv)
        {
            invisableCD = invisableTime;
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    void SetDefaultMaterial()
    {
        spriteRenderer.material = defaultMaterial;
    }

    public void PickupCoin(int coinType)
    {
        coinNum += Mathf.Max(1, (coinType - 1) * 5);
        coinNumTextGUI.text = coinNum.ToString();
        

        if (level <= levelupNeedCoin.Length)
        {
            if (coinNum >= levelupNeedCoin[level - 1])
            {
                coinNum -= levelupNeedCoin[level - 1];
                Levelup();
            }
            sliderExpProgress.value = (float)coinNum / (float)levelupNeedCoin[Math.Clamp(level - 1, 0, levelupNeedCoin.Length - 1)];
        }
    }

    private void Levelup()
    {
        level += 1;
        levelTextGUI.text = "Level:" + level;

        weaponUI.AddNewWeaponImage(level - 1);
        SetActiveWeapon(level - 1);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            TakeDamage(1);
        }
    }
}