﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathManager : MonoBehaviour
{
    //sounds
    public AudioSource audioSource;

    public GameObject target;

    public SpriteRenderer visualeffect;
    private Color spriteColour;

    private float vol = 0;

    private Vector3 increment;
    private Vector3 fullzoom;
    private Vector3 minzoom;
    private Vector3 currentzoom;
    private Vector3 pulseOffset;

    public float pulseSpeed = 2;
    public float pulseAmplitude = 1f;

    // Start is called before the first frame update
    void Start()
    {
        increment = new Vector3(0.0004025f, 0.000315f, 0f);
        fullzoom = new Vector3(2.3f, 1.8f, 1f);
        minzoom = new Vector3(1.15f, 0.9f, 1f);
        pulseOffset = new Vector3(0, 0, 0);
        currentzoom = fullzoom;

        //StartCoroutine(Pulse());

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //center on the player
        if (target != null)
        {
            Vector3 newPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            transform.position = newPosition;
        }

        audioSource.volume = vol;
        //transform.localScale = currentzoom + pulseOffset;
    }


    public void IncreaseVolume() //breathing gets louder as water level decreases
    {
        if (transform.localScale.x >= minzoom.x)
        {
            transform.localScale -= increment;
        }
    }


    public void DecreaseVolume(float refill) //breathing gets quieter when player collects a puddle. 20f
    {
        if (transform.localScale.x <= fullzoom.x)
        {
            transform.localScale += increment * refill * 100;
        }
        if (transform.localScale.x > fullzoom.x) transform.localScale = fullzoom;
    }


    public void SetVol(float value) //set the volume of the breathing
    {
        vol = value;
    }


    private IEnumerator Pulse()
    {
        while (true)
        {
            //Pulse effect
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
            pulseOffset = new Vector3(pulse, pulse, 0);

            //Wait for the next frame
            yield return null;
        }
    }


    public void Restart()
    {
        // Reset volume
        vol = 0;
    }


}