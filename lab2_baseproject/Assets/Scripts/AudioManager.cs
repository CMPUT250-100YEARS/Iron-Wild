using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    //public AudioSource audioSource2;

    // Start is called before the first frame update
    void Start()
    {
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
