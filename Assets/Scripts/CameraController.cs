using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject playerCharacter;
    private new Camera camera;

    public float cameraFollow = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCharacter = GameObject.FindWithTag("Player");
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 halfScreenSize = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 center = playerCharacter.transform.position;
        Vector2 dir = (Input.mousePosition - camera.WorldToScreenPoint(center)) / halfScreenSize;
        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);
        Vector2 offsetPos = cameraFollow * dir;
        Vector2 pos = center + offsetPos;
        transform.position = new Vector3(pos.x, pos.y, -10f);
        
        // camera.rect = Rect.MinMaxRect(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
