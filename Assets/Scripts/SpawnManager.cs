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
    private List<EnemyWave> enemyWaves = new List<EnemyWave>();
    [SerializeField] private bool readyToSpawn = false;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int currentSubwave = 0;

    // Start is called before the first frame update
    void Start()
    {
        // This is messy. Fix it.
        enemyWaves.Add(new EnemyWave());
        enemyWaves[0].subwaves.Add(new int[] {2,0,0,10});
        enemyWaves[0].subwaves.Add(new int[] {0,4,0,10});
        enemyWaves[0].subwaves.Add(new int[] {1,0,1,0});
        enemyWaves.Add(new EnemyWave());
        enemyWaves[1].subwaves.Add(new int[] {2,0,0,0});
        StartCoroutine(WaitForNextSpawn(10));
    }

    void Update()
    {
        if (readyToSpawn)
        {
            readyToSpawn = false;
            SpawnNextWave();
        }
    }

    void SpawnNextWave()
    {
        if (currentWave == enemyWaves.Count){
            // In theory you've won the game at this point
            return;
        }
        
        // spawn slow, medium and fast enemies according to subwave info
        SpawnEnemies(EnemyType.slow, enemyWaves[currentWave].subwaves[currentSubwave][0]);
        SpawnEnemies(EnemyType.medium, enemyWaves[currentWave].subwaves[currentSubwave][1]);
        SpawnEnemies(EnemyType.fast, enemyWaves[currentWave].subwaves[currentSubwave][2]);
        if (enemyWaves[currentWave].subwaves[currentSubwave][3] > 0){
            StartCoroutine(WaitForNextSpawn(enemyWaves[currentWave].subwaves[currentSubwave][3]));
        }
        else {
            StartCoroutine(WaitForNextSpawn(30));
        }
        currentSubwave++;   
        if (currentSubwave == enemyWaves[currentWave].subwaves.Count){
            currentWave ++;
            currentSubwave = 0;
        }
        // start waitfornextspawn coroutine if time is non-zero
        // if the time to next is 0, advance to next wave
        
    }

    IEnumerator WaitForNextSpawn(int delay)
    {
        yield return new WaitForSeconds(delay);
        readyToSpawn = true;
        
    }

    // Not sure about this. Worth refactoring when I implement the JSON but I need to plan it out a bit.
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

    [System.Serializable]
    class EnemyWave
    {
        public List<int[]> subwaves = new List<int[]>();
        
    }
}
