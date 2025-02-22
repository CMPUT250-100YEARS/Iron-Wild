﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button bossButton;
    public Button creditsButton;

    // Start is called before the first frame update
    void Start()
    {
        level1Button.onClick.AddListener(() => LoadLevel("Intro_2"));
        level2Button.onClick.AddListener(() => LoadLevel("City1"));
        bossButton.onClick.AddListener(() => LoadLevel("StairsCutScene"));
        creditsButton.onClick.AddListener(() => LoadLevel("Credits"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
