using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    private GameObject playerCharacter;
    private PlayerController playerController;
    private Transform muzzleSlot;
    private Transform shellSlot;
    private Animator animator;
    private CameraShake cameraShake;
    private AudioSource audioSource;
    
    public EmitterType emitterType = EmitterType.Single;
    [Range(1, 100)]
    public int bulletNum = 1;
    [Range(0f, 90.0f)]
    public float scatteringAngle = 60.0f;
    public float shootInverval = 0.1f;
    public GameObject bulletPrefab;
    public GameObject shellPrefab;
    public Texture2D cursorTexture2d;
    public Vector2 offsetRange = new Vector2(-5.0f, 5.0f);
    public float Recoil = 1f;
    public float RecoilForceOffset = 0.2f;
    public float shakeStrength = 2f;
    public AudioClip[] launchAudios;
    
    private Vector2 direction;
    private float cd = 0f;
    private bool isBack = true;
    private bool isCoolingDown = false;

    public enum EmitterType
    {
        Single,
        Volley,
        Repeat
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player");
        playerController = playerCharacter.GetComponent<PlayerController>();
        animator = gameObject.GetComponent<Animator>();
        muzzleSlot = transform.GetChild(0);
        shellSlot = transform.GetChild(1);
        cameraShake = Camera.main.GetComponent<CameraShake>();
        audioSource = GetComponent<AudioSource>();

        if (emitterType == EmitterType.Single)
        {
            bulletNum = 1;
        }
        
        if (cursorTexture2d != null)
        {
            Cursor.SetCursor(cursorTexture2d, Vector2.zero, CursorMode.Auto);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 枪支旋转
        Vector3 mousePos = Input.mousePosition;
        Vector3 objPos = Camera.main.WorldToScreenPoint(transform.position);
        direction = (mousePos - objPos).normalized;
        transform.right = playerController.GetPlayerFaceDir() * direction;
        
        if (isCoolingDown)
        {
            cd -= Time.deltaTime;
            if (cd <= 0f)
            {
                isCoolingDown = false;
            }
        }

        if (!isBack)
        {
            float posX = transform.localPosition.x + Time.deltaTime;
            if (posX >= 0f)
            {
                isBack = true;
                posX = 0f;
            }
            transform.localPosition = new Vector3(posX, 0f, 0f);
        }
    }
    
    public void SetFiring(bool isFiring)
    {
        if (isFiring)
        {
            if (emitterType == EmitterType.Repeat)
            {
                InvokeRepeating("Launch", 0, shootInverval);
            }
            else
            {
                if (!isCoolingDown)
                {
                    Launch();
                    isCoolingDown = true;
                    cd = shootInverval;
                }
            }
        }
        else
        {
            if (IsInvoking("Launch"))
            {
                CancelInvoke("Launch");
            }
        }
    }

    public void Launch()
    {
        if (bulletNum == 1)
        {
            ShootBullet(0);
        }
        else
        {
            float deltaAngle = scatteringAngle / (float)(bulletNum - 1);
            float angle;
            for (int i = 0; i < bulletNum; i++)
            {
                int multi = (i + (bulletNum % 2)) / 2;
                angle = Mathf.Pow(-1.0f, (float)(i)) * deltaAngle * ((float)multi + (bulletNum % 2 == 1 ? 0f : 0.5f));
                ShootBullet(angle);
            }
        }

        transform.localPosition = new Vector3(-RecoilForceOffset, 0f, 0f);
        isBack = false;
        animator.SetTrigger("Launch");
        playerController.AddRecoil(
            playerController.GetPlayerFaceDir() * -transform.right, 
            bulletPrefab.GetComponent<Bullet>().GetForce() * bulletNum);
        cameraShake.Shake(shakeStrength);
        
        audioSource.PlayOneShot(launchAudios[Random.Range(0, launchAudios.Length)]);
    }

    private void ShootBullet(float angle)
    {
        Quaternion offsetQuat = Quaternion.Euler(0f, 0f, Random.Range(offsetRange.x, offsetRange.y));
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 faceDir = quat * direction;
        GameObject bulletObject = Instantiate(bulletPrefab, (Vector2)muzzleSlot.position, quat * transform.rotation * offsetQuat);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.SetInverseBulletSprite(playerController.GetPlayerFaceDir() < 0);
        bullet.Launch(offsetQuat * faceDir);
        
        GameObject shellObject = Instantiate(shellPrefab, (Vector2)shellSlot.position, Quaternion.identity);
        Shell shell = shellObject.GetComponent<Shell>();
    }
}
