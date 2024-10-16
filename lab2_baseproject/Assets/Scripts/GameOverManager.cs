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

        restartButton.gameObject.SetActive(false); //???oct
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

        Debug.Log("GameOver GameOverFunc " + PlayerPrefs.GetInt("numHearts")); //???oct
        isGameOver = true;
        //Time.timeScale = 0f; // Pause the game: freezes player and water after ran out of water
        gameOverPanel.SetActive(true);
        restartButton.gameObject.SetActive(true); //???oct
    }

    public void RestartGame()  // restart button directly calls this
    {

        Debug.Log("RestartGame " + PlayerPrefs.GetInt("numHearts")); //???oct
        isGameOver = false;
        //Time.timeScale = 1f; // Resume the game
        gameOverPanel.SetActive(false); //???oct

        player.Restart(); // call the Restart method on the Player script
        water.Restart(); // call the Restart method on the WaterManager script//???oct3

        //Kid kid = FindObjectOfType<Kid>(); //????
        //kid.Restart(); //????

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name); //???oct
        SceneManager.LoadScene("SampleScene"); //???oct
        //SceneManager.LoadScene(0); //???oct

    }

    //public void PlayerLost() //???oct
    public void PlayerLost(string sceneName) //???oct
    {
        Debug.Log("GameOver PlayerLosthearts " + PlayerPrefs.GetInt("numHearts")); //???oct
        GameOverManager gameManager = FindObjectOfType<GameOverManager>();
        gameManager.GameOverFunct();
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.GameOver();
    }
}
