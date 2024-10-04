using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndTrigger : MonoBehaviour
{
    public GameObject speechBubble;
    public Text speechText;

    // Start is called before the first frame update
    void Start()
    {
        speechBubble.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShowSpeechBubble(float foodCount)
    {
        speechBubble.SetActive(true);

        if (foodCount < 2)
        {
            speechText.text = "I need more food!";
        } else
        {
            speechText.text = "Onto the next level!";
        }

        //player = FindObjectOfType<Player>();
        //Player player = gameObject.GetComponent<Player>(); //???
        //speechBubble.transform.position = playerPosition;// + new Vector3(-10f, -10f, 0);
        speechBubble.transform.position += new Vector3(50f, 50f, 0);

        Invoke("DoLaterFunction", 3f);  // wait 3 seconds before it's called

    }

    public void DoLaterFunction()
    {
        speechBubble.SetActive(false);
    }
}
