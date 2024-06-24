using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool isEnable = false;
    public float spawnInterval = 2f;
    public Vector2 spawnRange = new Vector2(2f, 3f);
    public GameObject EnemyPrefab;

    private float cd;
    private Vector2 spawnCenter;
    private GameObject playerCharacter;
    
    // Start is called before the first frame update
    void Start()
    {
        cd = spawnInterval;
        playerCharacter = GameObject.FindWithTag("Player");    
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnable)
        {
            return;
        }
        spawnCenter = playerCharacter.transform.position;

        cd -= Time.deltaTime;
        if (cd <= 0)
        {
            cd = spawnInterval;
            Vector2 dir = Random.insideUnitCircle;
            float radius = Mathf.Lerp(spawnRange.x, spawnRange.y, dir.magnitude);
            Vector2 spawnPos = spawnCenter + radius * dir;
            GameObject enemyObject = Instantiate(EnemyPrefab, spawnPos, Quaternion.identity);
            
            Vector2 moveDirection = spawnPos - spawnCenter;
            float angle = Vector2.Angle(Vector2.right, moveDirection);
            
            enemyObject.GetComponent<EnemyController>().SetFaceDir(angle > 90 ? 1 : -1);
        }
    }
}
