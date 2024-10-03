using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    public int maxLives = 2;
    public int currentLives;
    public Image[] heartImages;  // Array of heart images
    //public GameObject gameOverPanel; // Reference to the Game Over Ui Panel (optional)

    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;
        UpdateHearts();

        //// Optionally, disable the Game Over panel at the start
        //if (gameOverPanel != null)
        ////{
        //    gameOverPanel.SetActive(false);
        //}


    }

    // Update is called once per frame
    void Update()
    {

    }

    // Call this method to reduce a life
    public void LoseLife()
    {
        if (currentLives > 0)
        {
            Debug.Log("Lose a Life!");
            currentLives--;
            UpdateHearts();

            //if (currentLives <= 0){
            //    GameOver();
            //}
        }
        else
        {
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
        }
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
