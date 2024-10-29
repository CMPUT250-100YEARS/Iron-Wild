using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollText : MonoBehaviour
{

    public float scrollSpeed = 20f; // Speed of the scroll
    private RectTransform rectTransform; // Reference to the RectTransform
    private float startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startingPosition = rectTransform.anchoredPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        // Move the text upwards over time
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Reset position if it goes too far up
        if (rectTransform.anchoredPosition.y > startingPosition + 1000) // Adjust 1000 based on text length
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startingPosition);
        }
    }
}
