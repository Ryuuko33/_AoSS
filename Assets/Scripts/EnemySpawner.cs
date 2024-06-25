using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool isEnable = false;
    public float spawnInterval = 2f;
    public Vector2 spawnRange = new Vector2(2f, 3f);
    public GameObject[] EnemyPrefabs;
    public int[] EnemyWeights;

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
            int r = Random.Range(1, 101);
            for (int i = 0; i < EnemyWeights.Length; i++)
            {
                r -= EnemyWeights[i];
                if (r <= 0)
                {
                    GameObject enemyObject = Instantiate(EnemyPrefabs[i], spawnPos, Quaternion.identity);
            
                    Vector2 moveDirection = spawnPos - spawnCenter;
                    float angle = Vector2.Angle(Vector2.right, moveDirection);
            
                    enemyObject.GetComponent<EnemyController>().SetFaceDir(angle > 90 ? 1 : -1);
                    return;
                }
            }
        }
    }
}
