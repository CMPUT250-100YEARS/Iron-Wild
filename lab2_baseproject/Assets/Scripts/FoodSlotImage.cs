using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodSlotImage : MonoBehaviour
{
    public int maxSlots = 2;
    public Image[] SlotImages;  // Array of Slot images

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            SlotImages[i].enabled = true; // Enable slot images
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
