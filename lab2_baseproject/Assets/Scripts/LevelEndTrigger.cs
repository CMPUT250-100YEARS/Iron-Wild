using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    public GameObject speechBubble;
    public Text speechText;
    public string messageText;
    public bool firstTransform = true; 
    //public char letterText;

    // Start is called before the first frame update
    void Start()
    {
        speechBubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator AnimateSpeech(string message)
    {
        speechBubble.SetActive(true);
        speechText.text = "";
        //speechBubble.transform.position += new Vector3(50f, 50f, 0);
        if (firstTransform)
        {
            speechBubble.transform.position += new Vector3(75f, 100f, 0);  // 100f, 100f, 0
            firstTransform = false;
        }

        foreach (char letter in message)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        // if need more food, pause so speech bubble stays longer
        if (message == "I NEED MORE FOOD!")
        {
            yield return new WaitForSeconds(0.5f);
        }

        speechBubble.SetActive(false);
    }

    public void OnLevelComplete(string message)
    {
        StartCoroutine(AnimateSpeech(message));
    }

}