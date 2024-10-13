using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Button restartButton;
    public bool isGameOver = false;
    public Player player;
    public WaterManager water;
    //public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>(); 
        water = FindObjectOfType<WaterManager>();

        gameOverPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
    }

    public void GameOverFunct()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Pause the game
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()  // restart button directly calls this
    {
        isGameOver = false;
        Time.timeScale = 1f; // Resume the game

        player.Restart(); // call the Restart method on the Player script
        water.Restart(); // call the Restart method on the WaterManager script

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
