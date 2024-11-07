using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTransition: MonoBehaviour
{
    public AudioSource audioSource;
    //public AudioSource audioSource2;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource.volume = 0f;
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float x_pos = player.transform.position.x;
        if (x_pos < 58f) audioSource.volume = 0f;
        else if (x_pos > 83f) audioSource.volume = 0.55f;
        else
        {
            float vol = ((x_pos - 58) * 4) * 0.0055f;
            audioSource.volume = vol;
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
