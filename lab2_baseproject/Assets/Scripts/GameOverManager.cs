using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip deadSound;

    public GameObject gameOverPanel;
    public Button restartButton;
    public bool isGameOver = false;
    public Player player;
    public WaterManager water;
    //public Player playerScript;
    //public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>(); 
        water = FindObjectOfType<WaterManager>();

        gameOverPanel.SetActive(false);

        restartButton.gameObject.SetActive(false);
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

        Debug.Log("GameOver GameOverFunc " + PlayerPrefs.GetInt("numHearts"));
        isGameOver = true;
        // gunMoveable = false???
        audioSource.PlayOneShot(deadSound);
        Time.timeScale = 0f; // Pause the game: freezes player and water after ran out of water
        gameOverPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
        Player.gunMoveable = false;
    }

    public void RestartGame()  // restart button directly calls this
    {

        Debug.Log("RestartGame " + PlayerPrefs.GetInt("numHearts"));
        isGameOver = false;
        Time.timeScale = 1f; // Resume the game
        gameOverPanel.SetActive(false);

        player.Restart(); // call the Restart method on the Player script
        water.Restart(); // call the Restart method on the WaterManager script
        
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        string curScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(curScene);
        //SceneManager.LoadScene(0);
        Player.gunMoveable = true;

    }

    //public void PlayerLost()
    public void PlayerLost(string sceneName)
    {
        Debug.Log("GameOver PlayerLosthearts " + PlayerPrefs.GetInt("numHearts"));
        GameOverManager gameManager = FindObjectOfType<GameOverManager>();
        gameManager.GameOverFunct();
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.GameOver();
    }
}
