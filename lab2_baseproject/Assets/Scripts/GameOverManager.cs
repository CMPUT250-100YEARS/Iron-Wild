using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    //public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameOverFunct()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()  // restart button directly calls this
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayerLost()
    {
        GameOverManager gameManager = FindObjectOfType<GameOverManager>();
        gameManager.GameOverFunct();
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.GameOver();
    }
}
