using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroStory : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Scene Settings")]
    [SerializeField] private string sceneName;
  
    void OnEnable()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Scene name is not set!");
        }
        
    }
}
