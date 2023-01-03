using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    [SerializeField] TextMeshProUGUI survivalTimeText;
    [SerializeField] TextMeshProUGUI enemiesRemainingText;
    [SerializeField] TextMeshProUGUI waveNumberText;
    [SerializeField] GameObject waveIntro;
    AudioSource uiSource;
    public bool isIntroPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        // Not sure if this is the best way - was necessary for scene reload, might not be needed once the menu is in.
        MainManager.Instance.uiManager = GetComponent<UIManager>();
        uiSource = GetComponent<AudioSource>();
    }

    public void DisplayGameOver()
    {
        StartCoroutine(DisplayGameOver(3));
    }

    IEnumerator DisplayGameOver(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        gameOver.SetActive(true);
    }

    public void DisplayWaveIntro(int waveNumber)
    {
        isIntroPlaying = true;
        uiSource.Play();
        waveIntro.SetActive(true);
        StartCoroutine(IntroDelay(1, waveNumber));
        StartCoroutine(EndIntro(5));

    }

    IEnumerator IntroDelay(float secondsToDelay, int waveNumber)
    {
        yield return new WaitForSeconds(secondsToDelay);
        waveNumberText.text = $"WAVE {waveNumber}";
        uiSource.Play();
    }

    IEnumerator EndIntro(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        waveIntro.SetActive(false);
        waveNumberText.text = "";
        isIntroPlaying = false;
    }

    public void UpdateTime(string time)
    {
        survivalTimeText.text = $"Survived for {time}";
    }

    public void UpdateEnemies(int enemyCount)
    {
        enemiesRemainingText.text = $"{enemyCount} Enemies Remaining";
    }

}
