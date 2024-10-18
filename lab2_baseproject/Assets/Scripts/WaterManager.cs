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

    public float maxTime = 300f; //oct17
    //public float maxTime = 100f; //oct17
    public float timeLeft;
    public GameObject timesUpText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WaterManager Start maxTime !" + maxTime); //???oct17
        xPos = (Screen.width / 2) - 100;
        yPos = (Screen.height / 2) - 100;

        timeLeft = maxTime; //oct17
        Debug.Log("WaterManager Start timeLeft !" + timeLeft); //???oct17

    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            waterBar.fillAmount = timeLeft / maxTime; //oct17
        } else
        {
            Debug.Log("WaterManager Update timeLeft <=0 !"); //???oct

            timeLeft = maxTime; //???oct17 ELSE LOOPS FOREVER

            GameOverManager gameOver = FindObjectOfType<GameOverManager>();

            string sceneName = "CITY"; //???oct17
            gameOver.PlayerLost(sceneName); //???oct17

            //gameOver.PlayerLost("SampleScene"); //???oct17
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
        Debug.Log("WaterManager before IncreaseWater timeLeft !" + timeLeft); //???oct17
        timeLeft += amount;

        if (timeLeft > 100) //???oct17
        {
            timeLeft = 100; //???oct17
        }

        timeLeft = Mathf.Clamp(timeLeft, 0, 100); //ensure waterLevel stays between 0 and 100
        //waterBar.fillAmount = timeLeft / 50; //oct17 MUST HARDCODE???
        waterBar.fillAmount = timeLeft / maxTime; //oct17
        Debug.Log("WaterManager after IncreaseWater timeLeft !" + timeLeft); //???oct17
    }

    public void Restart()
    {
        Debug.Log("WaterManager Restart!"); //???oct
        // Reset water state
        xPos = (Screen.width / 2) - 100;
        yPos = (Screen.height / 2) - 100;

        timeLeft = maxTime; //oct17
    }
}