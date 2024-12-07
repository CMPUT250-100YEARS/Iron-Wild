using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished { get; protected set; }

        //fade in and out image
        //[SerializeField] private CanvasGroup backgroundCanvasGroup; // Reference to CanvasGroup for fade effect
        //[SerializeField] private float fadeDuration = 1f; // Duration for fade in/out

        protected IEnumerator WriteText (string input, Text textHolder, float baseDelay, float delayBetweenLines )
        {

            //***************

            //***************

            //textHolder.color = textColor;
            //textHolder.font = textFont;
            finished = false; // Reset finished status
            textHolder.text = ""; // Clear text before starting
            float delay = baseDelay;

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                if (Input.GetKey(KeyCode.Space))
                {
                    delay = Mathf.Max(0.01f, delay - 0.01f);
                }
                else
                {
                    delay = baseDelay;
                }
                //play letter sound;
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitUntil(() => Input.GetMouseButton(0));
            //yield return new WaitForSeconds(delayBetweenLines);

            finished = true;
        }
    }
}


