using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button bossButton;

    // Start is called before the first frame update
    void Start()
    {
        level1Button.onClick.AddListener(() => LoadLevel("SampleScene"));
        level2Button.onClick.AddListener(() => LoadLevel("CITY"));
        bossButton.onClick.AddListener(() => LoadLevel("RoofTop"));
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
