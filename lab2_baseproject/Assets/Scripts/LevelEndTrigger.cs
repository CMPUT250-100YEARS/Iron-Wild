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
        //speechBubble.transform.position += new Vector3(50f, 50f, 0);
        speechBubble.transform.position += new Vector3(100f, 100f, 0);

        foreach (char letter in message)
        {
            speechText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }

        //yield return new WaitForSeconds(2f);

        speechBubble.SetActive(false);
    }

    public void OnLevelComplete(string message)
    {
        StartCoroutine(AnimateSpeech(message));
    }

}