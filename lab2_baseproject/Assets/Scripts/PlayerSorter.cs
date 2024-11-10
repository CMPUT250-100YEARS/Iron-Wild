using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSorter : MonoBehaviour
{
    private SpriteRenderer playerRenderer;
    private int foregroundLayerOrder = 8; // Set to a value that fits your sorting layer setup

    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is below or above solid objects in the foreground
        if (transform.position.y > playerRenderer.bounds.min.y)
        {
            // Player is "in front" of foreground objects
            playerRenderer.sortingOrder = foregroundLayerOrder + 1;
        }
        else
        {
            // Player is "behind" foreground objects
            playerRenderer.sortingOrder = foregroundLayerOrder - 1;
        }
    }
}
