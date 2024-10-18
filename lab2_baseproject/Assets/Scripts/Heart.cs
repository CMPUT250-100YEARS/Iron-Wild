using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public int maxLives = 6;
    public int currentLives; //??? rid static
    public Image[] heartImages;  // Array of heart images
    //public GameObject gameOverPanel; // Reference to the Game Over Ui Panel (optional)

    // Start is called before the first frame update
    void Start()
    {
        //currentLives = PlayerPrefs.GetInt("numHearts");
        //UpdateHearts();

        //// Optionally, disable the Game Over panel at the start
        //if (gameOverPanel != null)
        ////{
        //    gameOverPanel.SetActive(false);
        //}


    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("numHearts") <= 0) //???oct
        {
            Debug.Log("#13hearts before" + PlayerPrefs.GetInt("numHearts")); //???oct
            PlayerPrefs.SetInt("numHearts", maxLives); //???oct
            Debug.Log("#13hearts after" + PlayerPrefs.GetInt("numHearts")); //???oct
            GameOverManager gameOver = FindObjectOfType<GameOverManager>(); //???oct
            Debug.Log("#13hearts get gameover object" + PlayerPrefs.GetInt("numHearts")); //???oct
            string sceneName = "CITY"; //???oct2
            gameOver.PlayerLost(sceneName); //???oct2
            //gameOver.PlayerLost("SampleScene"); //???oct2
            Debug.Log("#13hearts call player lost" + PlayerPrefs.GetInt("numHearts")); //???oct
        }

    }

    public void InitializeHearts()
    {
        currentLives = PlayerPrefs.GetInt("numHearts");
        Debug.Log("#3hearts" + PlayerPrefs.GetInt("numHearts"));
    }


    // Call this method to reduce a life
    public void LoseLife()
    {
        if (currentLives > 0)
        {
            Debug.Log("Lose a Life!");
            currentLives--;
            PlayerPrefs.SetInt("numHearts", currentLives);
            PlayerPrefs.Save();
            UpdateHearts();
            Debug.Log("#4hearts" + PlayerPrefs.GetInt("numHearts"));

            //// check if game over
            //if (currentLives <= 0)
            //{
            //    GameOverManager gameOver = FindObjectOfType<GameOverManager>();
            //    gameOver.PlayerLost();
            //}
        }
        else
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Cannot Lose any more Lives!");
        }
    }

    // Method to update heart images based on current lives
    public void UpdateHearts()
    {
        Debug.Log("Update Hearts!");
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < currentLives); // Enable or disable heart images
            Debug.Log("Update Hearts!" + i); //???oct
        }
        Debug.Log("#7hearts" + currentLives); //???
    }

    // Method to handle game over logic
    //public void GameOver()
    //{
    //    // Optionally activate a Game Over UI Panel
    //    if (gameOverPanel != null)
    //    {
    //        gameOverPanel.SetActive(true);
    //    }

    //    //We can also implement additional game over logic here
    //    Debug.Log("Game Over! All lives lost.");
    //}

    public void TakeDamage()
    {
        LoseLife();
    }
}
