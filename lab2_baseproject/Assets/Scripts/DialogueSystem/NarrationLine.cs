using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class NarrationLine : DialogueBaseClass
    {
        private Text textHolder;

        //[Header ("Text Options")]
        [SerializeField] private string input;
        //[SerializeField] private Color textColor;
        //[SerializeField] private Font textFont;

        [Header("Time parameters")]
        [SerializeField] private float delay;
        [SerializeField] private float delayBetweenLines;

        private void Awake()
        {
            textHolder = GetComponent<Text>();
            textHolder.text = "";

            //StartCoroutine(WriteText(input, textHolder, delay));

        }

        private void OnEnable()
        {
            //**
            finished = false;
            StartCoroutine(WriteText(input, textHolder, delay, delayBetweenLines));
        }


    }
}
