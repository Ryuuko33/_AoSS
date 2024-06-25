using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    private new Camera camera;

    public float strength = 2f;
    public float duration = 0.2f;
    public float shakeFps = 45f;

    private float defaultStrength = 0f;
    private float fps;
    private float shakeTime = 0.0f;
    private float frameTime = 0.03f;
    private float shakeDelta = 0.005f;

    private Rect changeRect;
    private bool isShaking = false;

    void Awake()
    {
        camera = GetComponent<Camera>();
        changeRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        shakeTime = duration;
        fps = shakeFps;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            if (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                if (shakeTime <= 0)
                {
                    changeRect.xMin = 0.0f;
                    changeRect.yMin = 0.0f;
                    camera.rect = changeRect;
                    isShaking = false;
                    shakeTime = duration;
                    fps = shakeFps;
                    frameTime = 0.03f;
                    shakeDelta = 0.005f;
                    if (strength != defaultStrength)
                    {
                        strength = defaultStrength;
                    }
                }
                else
                {
                    frameTime += Time.deltaTime;
                    
                    if (frameTime > 1.0 / fps)
                    {
                        frameTime = 0;
                        changeRect.xMin = shakeDelta * (-1.0f + strength * Random.value);
                        changeRect.yMin = shakeDelta * (-1.0f + strength * Random.value);
                        camera.rect = changeRect;
                    }
                }
            }
        }
    }

    public void Shake(float strenValue)
    {
        strength = strenValue;
        shakeTime = duration;
        isShaking = true;
    }
}