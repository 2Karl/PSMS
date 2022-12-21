using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance {get; private set;}
    public enum GameState{mainMenu, pauseMenu, playing, sceneTransition, death}
    [SerializeField] public GameState gameState {get; private set;} = GameState.playing;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.playing) {
            gameState = GameState.pauseMenu;
            Time.timeScale = 0f;
            //Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && gameState == GameState.pauseMenu) {
            gameState = GameState.playing;
            Time.timeScale = 1f;
            //Cursor.visible = false;
        }   
    }

    public void GameOver()
    {
        gameState = GameState.death;
    }
}
