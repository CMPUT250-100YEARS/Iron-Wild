using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    //public AudioSource audioSource2;

    private bool forestlevel = false;
    private GameObject player;

    public AudioClip altmusic;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (audioSource != null)
        {
            audioSource.Play();
        }

        Scene currentScene = SceneManager.GetActiveScene();
        
        if (currentScene.name == "SampleScene")
        {
            forestlevel = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (forestlevel)
        {
            float x_pos = player.transform.position.x;
            if (x_pos < 62f) audioSource.volume = 0.55f;
            else if (x_pos > 87f) audioSource.volume = 0f;
            else
            {
                float vol = (100 - (x_pos - 62) * 4) * 0.0055f;
                audioSource.volume = vol;
            }
        }

    }

    public void GameOver()
    {
        if (audioSource != null)// && (audioSource2 != null)) 
        {
            audioSource.Stop(); 
            //audioSource2.Play(); 
        }
    }
}
