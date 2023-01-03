using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance {get; private set;}
    public enum GameState{mainMenu, pauseMenu, playing, sceneTransition, death, preWave, postWave}
    [SerializeField] public GameState gameState {get; private set;} = GameState.preWave;
    private AudioSource mainSource;
    [SerializeField] public int enemyCount {get; private set;} = 0;
    public UIManager uiManager {private get; set;}
    private float survivalTime = 0;
    private int waveNumber = 0;

    void Awake()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        mainSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (gameState == GameState.playing){
            UpdateTime();
        }
        // farm out the pause menu stuff to another function
        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.playing) {
            gameState = GameState.pauseMenu;
            Time.timeScale = 0f;
            mainSource.Pause();
            //Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.pauseMenu) {
            gameState = GameState.playing;
            Time.timeScale = 1f;
            mainSource.UnPause();
            //Cursor.visible = false;
        }
        if(gameState == GameState.death && Input.GetKeyDown(KeyCode.Space)){
            Reset();
        }

    }

    void UpdateTime() {
        survivalTime += Time.deltaTime;
        TimeSpan formattedTime = TimeSpan.FromSeconds(survivalTime);
        uiManager.UpdateTime(formattedTime.ToString("mm':'ss'.'fff"));
    }

    public void AddEnemy()
    {
        enemyCount++;
        uiManager.UpdateEnemies(enemyCount);
    }

    public void RemoveEnemy()
    {
        enemyCount--;
        uiManager.UpdateEnemies(enemyCount);
    }

    public void GameOver()
    {
        gameState = GameState.death;
        uiManager.DisplayGameOver();
    }

    void Reset()
    {
        waveNumber = 0;
        survivalTime = 0;
        enemyCount = 0;
        gameState = GameState.preWave;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextWaveIntro()
    {
        waveNumber++;
        gameState = GameState.preWave;
        uiManager.DisplayWaveIntro(waveNumber);
        StartCoroutine(WaitUntilIntroFinished());
    }

    IEnumerator WaitUntilIntroFinished()
    {
        yield return new WaitWhile(() => uiManager.isIntroPlaying);
        gameState = GameState.playing; 
    }
}
