using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private enum Location {top, bottom, left, right};
    private enum EnemyType {slow, medium, fast, midBoss, endBoss};
    private float xRange = 20;
    private float zRange = 15;
    EnemyWaves enemyWaves = new EnemyWaves();
    [SerializeField] private bool isReadyToSpawn = false;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int currentSubWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        LoadWaves();
        StartCoroutine(WaitForNextWave(2));
    }

    void LoadWaves()
    {
        string path = Application.persistentDataPath + "/waves.json";
        if(!File.Exists(path)){
            Debug.Log("No custom wave info in config path, loading built-in");
            path = Application.dataPath + "/Config/waves.json";
        }
        string json = File.ReadAllText(path);
        enemyWaves = JsonUtility.FromJson<EnemyWaves>(json);
    }

    void Update()
    {
        if (MainManager.Instance.gameState == MainManager.GameState.death) {
            StopSpawning();
        }
        if (isReadyToSpawn)
        {
            isReadyToSpawn = false;
            SpawnNextWave();
        }
    }

    IEnumerator WaitForNextWave(float secondsToWait)
    {
        Debug.Log("OK");
        yield return new WaitForSeconds(secondsToWait);
        MainManager.Instance.NextWaveIntro();
        StartCoroutine(WaitForNextSpawn(5));
    }

    SubWave GetCurrentSubWave()
    {
        if(currentWave >= enemyWaves.waves.Count){
            Debug.Log("Ran out of waves");
            return null;
        }
        if(currentSubWave >= enemyWaves.waves[currentWave].subWaves.Count){
            Debug.LogError("Ran out of subwaves - this shouldn't happen");
            return null;
        }
        return enemyWaves.waves[currentWave].subWaves[currentSubWave];
    }

    void SpawnNextWave()
    {
        
        SubWave subWave = GetCurrentSubWave();
        if (subWave == null){
            // TODO: implement wave transition
            Debug.Log("Either all waves are done or something's gone wrong");
            return;
        }
        // spawn slow medium and fast enemies according to subwave info
        SpawnEnemies(EnemyType.slow, subWave.slow);
        SpawnEnemies(EnemyType.medium, subWave.medium);
        SpawnEnemies(EnemyType.fast, subWave.fast);
        
        if (subWave.delay == 0){
            // this is the last subwave of the wave so we should wait for all enemies to be destroyed
            currentSubWave = 0;
            currentWave++;
            Debug.Log("Wait for enemies to be wiped out");
            StartCoroutine(WaitUntilEnemiesWipedOut());
        }
        else {
            currentSubWave++;
            StartCoroutine(WaitForNextSpawn(subWave.delay));
        }
    }

    bool IsEnemyAlive(){
        return MainManager.Instance.enemyCount > 0;
    }

    IEnumerator WaitUntilEnemiesWipedOut()
    {
        Debug.Log("Wait coroutine triggered");
        yield return new WaitWhile(() => IsEnemyAlive());
        StartCoroutine(WaitForNextWave(2));
    }

    IEnumerator WaitForNextSpawn(int delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        isReadyToSpawn = true;
    }

    void SpawnEnemy(EnemyType enemyType, Vector3 spawnLocation)
    {
        GameObject enemy = enemyPrefabs[(int)enemyType];
        Instantiate(enemy, spawnLocation, enemy.transform.rotation);
        MainManager.Instance.AddEnemy();
    }

    void SpawnEnemy(EnemyType enemyType)
    {
        Vector3 spawnLocation = GenerateEnemySpawnLocation();
        GameObject enemy = enemyPrefabs[(int)enemyType];
        Instantiate(enemy, spawnLocation, enemy.transform.rotation);
        MainManager.Instance.AddEnemy();
    }

    void SpawnEnemies(EnemyType enemyType, int numberOfEnemies)
    {
        for(int i=0; i < numberOfEnemies; i++){
            SpawnEnemy(enemyType);
        }
    }

    Vector3 GenerateEnemySpawnLocation()
    {
        Location location = (Location)Random.Range(0, 4);
        Vector3 spawnLocation = new Vector3(0, 1, 0);
        switch (location) {
            case Location.top:
                spawnLocation.z = zRange;
                spawnLocation.x = Random.Range(-xRange, xRange);
            break;
            case Location.bottom:
                spawnLocation.z = -zRange;
                spawnLocation.x = Random.Range(-xRange, xRange);
            break;
            case Location.left:
                spawnLocation.z = Random.Range(-zRange, zRange);
                spawnLocation.x = -xRange;
            break;
            case Location.right:
                spawnLocation.z = Random.Range(-zRange, zRange);
                spawnLocation.x = xRange;
            break;
            default:
            break;
        }
        return spawnLocation;
    }

    void StopSpawning()
    {
        StopAllCoroutines();
        isReadyToSpawn = false;
    }
    

    // Wrapper classes for JSON handling because you can't serialize lists of lists, annoyingly
    [System.Serializable]
    class SubWave 
    {
        public int slow;
        public int medium;
        public int fast;
        public int delay;
    }
    [System.Serializable]
    class EnemyWave
    {
        public List<SubWave> subWaves = new List<SubWave>();
        
    }
    [System.Serializable]
    class EnemyWaves
    {
        public List<EnemyWave> waves = new List<EnemyWave>();
    }
}
