using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public Button menuButton;

    // Start is called before the first frame update
    void Start()
    {
        menuButton.onClick.AddListener(() => LoadLevel("Start Menu"));
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
