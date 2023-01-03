using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance {get; private set;}
    public enum GameState{mainMenu, pauseMenu, playing, sceneTransition, death}
    [SerializeField] public GameState gameState {get; private set;} = GameState.playing;
    private AudioSource musicSource;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        musicSource = GetComponent<AudioSource>();
        if(gameState == GameState.playing) {
            musicSource.Play();
        }
        //Cursor.visible = false;
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.playing) {
            gameState = GameState.pauseMenu;
            Time.timeScale = 0f;
            musicSource.Pause();
            //Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.pauseMenu) {
            gameState = GameState.playing;
            Time.timeScale = 1f;
            musicSource.UnPause();
            //Cursor.visible = false;
        }   
    }

    public void SlowSounds()
    {
        Debug.Log(enemies.Count);
        foreach(GameObject enemy in enemies){
            AudioSource enemySource = enemy.GetComponent<AudioSource>();
            enemySource.pitch = 0.5f;
        }
    }

    public void RestoreSounds()
    {
        foreach(GameObject enemy in enemies){
            AudioSource enemySource = enemy.GetComponent<AudioSource>();
            enemySource.pitch = 1f;
        }
    }

    public void GameOver()
    {
        gameState = GameState.death;
    }
}
