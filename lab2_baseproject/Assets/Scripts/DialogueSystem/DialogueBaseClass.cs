using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished { get; protected set; }
        protected IEnumerator WriteText (string input, Text textHolder, float delay, float delayBetweenLines )
        {


            //textHolder.color = textColor;
            //textHolder.font = textFont;

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                //play letter sound;
                yield return new WaitForSeconds(delay);
            }

            //yield return new WaitUntil(() => Input.GetMouseButton(0));
            yield return new WaitForSeconds(delayBetweenLines);

            finished = true;
        }
    }
}


