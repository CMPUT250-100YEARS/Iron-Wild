using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    public GameObject speechBubble;
    public Text speechText;
    public string messageText;
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
        speechBubble.transform.position += new Vector3(50f, 50f, 0);

        foreach (char letter in message)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);

        speechBubble.SetActive(false);
    }

    public void OnLevelComplete(string message)
    {
        StartCoroutine(AnimateSpeech(message));
    }

    //public void ShowSpeechBubble(float foodCount)
    //{
    //    speechBubble.SetActive(true);

    //    if (foodCount < 2)
    //    {
    //        messageText = "I need more food!";
    //        //speechText.text = "I need more food!";
    //    } else
    //    {
    //        messageText = "Onto the next level!";
    //        //speechText.text = "Onto the next level!";
    //    }

    //    AnimateSpeech(messageText);

    //    //player = FindObjectOfType<Player>();
    //    //Player player = gameObject.GetComponent<Player>(); //???
    //    //speechBubble.transform.position = playerPosition;// + new Vector3(-10f, -10f, 0);
    //    speechBubble.transform.position += new Vector3(50f, 50f, 0);

    //    //Invoke("DoLaterFunction", 3f);  // wait 3 seconds before it's called

    //}

    //public void AnimateSpeech(string message)
    //{
    //    speechText.text = "";
    //    foreach (char letter in message)
    //    {
    //        letterText = letter;
    //        //speechText.text += letter;  // add one letter at a time
    //        Invoke("AddLetter", 0.1f);
    //        //yield return new WaitForSeconds(0.1f);  // wait before adding next letter
    //    }
    //}


    //public void AddLetter()
    //{
    //    speechText.text += letterText;
    //}

    //public void DoLaterFunction()
    //{
    //    speechBubble.SetActive(false);
    //}
}
