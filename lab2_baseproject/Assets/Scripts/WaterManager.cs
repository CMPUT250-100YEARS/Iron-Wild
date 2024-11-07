using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaterManager : MonoBehaviour
{
    public Image waterBar;
    public float waterLevel = 100f;
    public float timePassed = 0f;
    public float decreaseTime = 300f;
    public float speed = 0.001f;
    public float xPos;
    public float yPos;

    //sounds
    public AudioClip warning;
    public AudioClip breathing;

    public float maxTime = 100f;
    //public float maxTime = 50f;
    public float timeLeft;
    public GameObject timesUpText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WaterManager Start maxTime !" + maxTime);
        xPos = (Screen.width / 2) - 100;
        yPos = (Screen.height / 2) - 100;

        timeLeft = maxTime;
        Debug.Log("WaterManager Start timeLeft !" + timeLeft);


    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            waterBar.fillAmount = timeLeft / maxTime;
        } else
        {
            Debug.Log("WaterManager Update timeLeft <=0 !");

            timeLeft = maxTime; //Prevents Infinite Loop

            GameOverManager gameOver = FindObjectOfType<GameOverManager>();

            string sceneName = "SampleScene";
            gameOver.PlayerLost(sceneName);

            //gameOver.PlayerLost("SampleScene");
            //Heart heart = FindObjectOfType<Heart>().LoseLife();
        }


        //if (waterLevel <= 0)
        //{
        //    //Application.LoadLevel(Application.loadedLevel);
        //    //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

    }

    public void DecreaseWater(float amount)
    {
        waterLevel -= amount;
        waterBar.fillAmount = waterLevel / 100f;
    }


    //public void SetWater()
    //{
    //    waterBar.fillAmount = 0f;
    //}


    public void IncreaseWater(float amount)
    {
        Debug.Log("WaterManager before IncreaseWater timeLeft !" + timeLeft);
        timeLeft += amount;

        if (timeLeft > 100)
        {
            timeLeft = 100;
        }

        timeLeft = Mathf.Clamp(timeLeft, 0, 100); //ensure waterLevel stays between 0 and 100
     
        waterBar.fillAmount = timeLeft / maxTime;

        Debug.Log("WaterManager after IncreaseWater timeLeft !" + timeLeft);
    }

    public void Restart()
    {
        Debug.Log("WaterManager Restart!");
        // Reset water state
        xPos = (Screen.width / 2) - 100;
        yPos = (Screen.height / 2) - 100;

        timeLeft = maxTime;
    }


}