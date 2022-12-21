using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private enum Location {top, bottom, left, right};
    private enum EnemyType {slow, medium, fast};
    private float xRange = 20;
    private float zRange = 15;
    EnemyWaves enemyWaves = new EnemyWaves();
    [SerializeField] private bool readyToSpawn = false;
    [SerializeField] private int currentWave = 0;
    [SerializeField] private int currentSubwave = 0;

    // Start is called before the first frame update
    void Start()
    {
        LoadWaves();
        StartCoroutine(WaitForNextSpawn(10));
    }

    void LoadWaves()
    {
        string path = Application.persistentDataPath + "/waves.json";
        if(!File.Exists(path)){
            path = Application.dataPath + "/Config/waves.json";
            Debug.Log("can't find file in config path");
        }
        string json = File.ReadAllText(path);
        enemyWaves = JsonUtility.FromJson<EnemyWaves>(json);
    }

    void Update()
    {
        if (readyToSpawn)
        {
            readyToSpawn = false;
            SpawnNextWave();
        }
    }

    SubWave GetCurrentSubWave()
    {
        if(currentWave >= enemyWaves.waves.Count){
            Debug.Log("Ran out of waves");
            return null;
        }
        if(currentSubwave >= enemyWaves.waves[currentWave].subwaves.Count){
            Debug.LogError("Ran out of subwaves - this shouldn't happen");
            return null;
        }
        return enemyWaves.waves[currentWave].subwaves[currentSubwave];
    }

    void SpawnNextWave()
    {

        SubWave subWave = GetCurrentSubWave();
        if (subWave == null){
            Debug.Log("Either all waves are done or something's gone wrong");
            return;
        }
        // spawn slow medium and fast enemies according to subwave info
        SpawnEnemies(EnemyType.slow, subWave.slow);
        SpawnEnemies(EnemyType.medium, subWave.medium);
        SpawnEnemies(EnemyType.fast, subWave.fast);
        
        if (subWave.delay == 0){
            // this is the last subwave of the wave so we should wait for all enemies to be destroyed
            currentSubwave = 0;
            currentWave++;
            StartCoroutine(WaitUntilWipedOut());
        }
        else {
            currentSubwave++;
            StartCoroutine(WaitForNextSpawn(subWave.delay));
        }
        
    }

    bool IsEnemyAlive(){
        return(GameObject.FindGameObjectsWithTag("Enemy").Length > 0);
    }

    IEnumerator WaitUntilWipedOut()
    {
        yield return new WaitWhile(() => IsEnemyAlive());
        // This should trigger the next wave announcement once this is implemented
        readyToSpawn = true;
    }

    IEnumerator WaitForNextSpawn(int delay)
    {
        yield return new WaitForSeconds(delay);
        readyToSpawn = true;
        
    }

    // Not sure about this. Worth refactoring when I implement the JSON but I need to plan it out a bit.
    // Better to have a "SpawnEnemy" method and wrap it in a loop in SpawnEnemies? hmmm.
    // Should probably have a GenerateRandomPosition function too

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
        public List<SubWave> subwaves = new List<SubWave>();
        
    }
    [System.Serializable]
    class EnemyWaves
    {
        public List<EnemyWave> waves = new List<EnemyWave>();
    }
}
