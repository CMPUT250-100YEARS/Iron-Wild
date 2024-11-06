using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodImage : MonoBehaviour
{
    public int maxFoods = 5;
    public int currentFoods = 0;
    public Image[] FoodImages;  // Array of Food images

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxFoods; i++)
        {
            FoodImages[i].enabled = false; // Disable food images
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    // Call this method to reduce a life
    public void FoundFoods()
    {
        if (currentFoods < 2)
        {
            Debug.Log("Found foods!");
            currentFoods++;
            UpdateFoods();
        }
    }

    public void UpdateFoods()
    {
        //currentFoods++;
        Debug.Log("Update Foods!");
        for (int i = 0; i < FoodImages.Length; i++)
        {
            FoodImages[i].enabled = (i < currentFoods); // Enable or disable food images
        }


    }
}
