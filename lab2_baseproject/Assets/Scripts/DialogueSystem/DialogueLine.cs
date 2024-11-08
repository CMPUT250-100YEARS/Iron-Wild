using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        private Text textHolder;

        //[Header ("Text Options")]
        [SerializeField] private string input;
        //[SerializeField] private Color textColor;
        //[SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;
        [SerializeField] private float delayBetweenLines;

        //[Header("Character Image")]
        //[SerializeField] private Sprite characterSprite;
        //[SerializeField] private Image imageHolder;

        [Header("Background Settings")]
        [SerializeField] private Sprite backgroundSprite; // Background image for this line
        [SerializeField] private Image backgroundImageHolder; //UI image


        private void Awake()
        {
            textHolder = GetComponent<Text>();
            textHolder.text = "";

            //StartCoroutine(WriteText(input, textHolder, delay));

        }

        private void OnEnable()
        {
            
            finished = false;

            //**
            // Set the background image if specified
            if (backgroundImageHolder != null && backgroundSprite != null)
            {
                backgroundImageHolder.sprite = backgroundSprite;
                backgroundImageHolder.enabled = true; // Ensure the image is enabled
            }
            //**

            StartCoroutine(WriteText(input, textHolder, delay, delayBetweenLines));
        }
        
       
    }
}

