using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pause;
    public Button pauseButton;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}
