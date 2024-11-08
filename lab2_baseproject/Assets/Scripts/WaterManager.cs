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
    public AudioSource audioSource;
    public AudioClip warning;

    private bool belowthird = false;
    private bool belowquarter = false;
    private bool beloweighth = false;
    private bool belowsixteen = false;

    public float maxTime = 100f;
    //public float maxTime = 50f;
    public float timeLeft;
    public GameObject timesUpText;
    private BreathManager breathing;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("WaterManager Start maxTime !" + maxTime);
        xPos = (Screen.width / 2) - 100;
        yPos = (Screen.height / 2) - 100;

        breathing = FindObjectOfType<BreathManager>();

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
            if (belowquarter) breathing.IncreaseVolume();
            PlayWaterSounds();
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

        if (belowquarter) //Update breathing
            {
                breathing.DecreaseVolume(amount);
            }

        timeLeft += amount;

        if (timeLeft > 100)
        {
            timeLeft = 100;
        }

        timeLeft = Mathf.Clamp(timeLeft, 0, 100); //ensure waterLevel stays between 0 and 100
     
        waterBar.fillAmount = timeLeft / maxTime;

        Debug.Log("WaterManager after IncreaseWater timeLeft !" + timeLeft);
    }

    public void PlayWaterSounds()
    {
        //play sounds if relevant, signal for breathing to start and stop
        if (timeLeft <= 6f)
        {
            if (!belowsixteen)
            {
                audioSource.PlayOneShot(warning);
                belowsixteen = true;
                beloweighth = true;
                belowquarter = true;
                belowthird = true;
            }
        }
        else if (timeLeft <= 12f)
        {
            if (!beloweighth)
            {
                audioSource.PlayOneShot(warning);
                beloweighth = true;
                belowquarter = true;
                belowthird = true;
            }
            else belowsixteen = false;
        }
        else if (timeLeft <= 25f)
        {
            if (!belowquarter)
            {
                belowquarter = true;
                belowthird = true;
            }
            else beloweighth = false;
        }
        else if (timeLeft <= 38f)
        {
            if (!belowthird)
            {
                audioSource.PlayOneShot(warning);
                belowthird = true;
            }
            else
            {
                beloweighth = false;
                belowquarter = false;
            }
        }
        else
        {
            beloweighth = false;
            belowquarter = false;
            belowthird = false;
        }
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