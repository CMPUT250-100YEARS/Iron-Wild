using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalMenuControl : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    public void LoadScene(string sceneName)
    {
        // Loads the scene with the given name
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Exits the application
        Debug.Log("Quit Game"); 
        Application.Quit();
    }
}
