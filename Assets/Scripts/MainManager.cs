using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance {get; private set;}
    public enum GameState{mainMenu, pauseMenu, playing, sceneTransition, death}
    [SerializeField] public GameState gameState {get; private set;} = GameState.playing;
    private AudioSource mainSource;
    public int enemyCount {get; private set;} = 0;
    [SerializeField] private TextMeshProUGUI enemyRemainingText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;
    [SerializeField] private GameObject gameOver;
    
    private float survivalTime = 0;


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        mainSource = GetComponent<AudioSource>();
        // This should be in a separate function which trippers on game start
        // I'll sort it out when I have a menu
        if(gameState == GameState.playing) {
            mainSource.Play();
            gameOver.SetActive(false);
            enemyRemainingText.text = $"{enemyCount} Enemies Remaining";
        }
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.playing){
            UpdateTime();
        }
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
    }

    void UpdateTime() {
        survivalTime += Time.deltaTime;
        TimeSpan formattedTime = TimeSpan.FromSeconds(survivalTime);
        
        survivalTimeText.text = $"Survived for {formattedTime.ToString("mm':'ss'.'fff")}";
    }

    public void AddEnemy()
    {
        enemyCount++;
        enemyRemainingText.text = $"{enemyCount} Enemies Remaining";
    }

    public void RemoveEnemy()
    {
        enemyCount--;
        enemyRemainingText.text = $"{enemyCount} Enemies Remaining";
    }

    public void GameOver()
    {
        
        gameState = GameState.death;
        Debug.Log("Paul Smith did not Survive!!!!!");
        StartCoroutine(DisplayGameOver(3));
    }

    IEnumerator DisplayGameOver(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        gameOver.SetActive(true);
        Debug.Log("time out");
    }
}
