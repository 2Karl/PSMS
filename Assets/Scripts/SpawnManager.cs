using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private enum Location {top, bottom, left, right};
    private enum EnemyType {slow, medium, fast};
    private float xRange = 20;
    private float zRange = 15;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies(EnemyType.slow, 2);
        SpawnEnemies(EnemyType.medium, 4);
        SpawnEnemies(EnemyType.fast, 3);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void SpawnEnemies(EnemyType enemyType, int numberOfEnemies)
    {
        for(int i=0; i < numberOfEnemies; i++){
            Location location = (Location)Random.Range(0, 4);
            Vector3 spawnPos = new Vector3(0, 1, 0);
            switch (location) {
                case Location.top:
                    spawnPos.z = zRange;
                    spawnPos.x = Random.Range(-xRange, xRange);
                break;
                case Location.bottom:
                    spawnPos.z = -zRange;
                    spawnPos.x = Random.Range(-xRange, xRange);
                break;
                case Location.left:
                    spawnPos.z = Random.Range(-zRange, zRange);
                    spawnPos.x = -xRange;
                break;
                case Location.right:
                    spawnPos.z = Random.Range(-zRange, zRange);
                    spawnPos.x = xRange;
                break;
                default:
                break;
            }
        
            Instantiate(enemyPrefabs[(int)enemyType], spawnPos, enemyPrefabs[(int)enemyType].transform.rotation);
        }
    }
}
