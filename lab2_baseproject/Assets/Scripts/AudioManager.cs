using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //music
    public AudioSource audioSource;
    public AudioClip music;
 
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = music;
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

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
