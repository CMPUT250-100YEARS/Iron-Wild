
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{

    private Enemy parent;
    // Start is called before the first frame updates
    void Start()
    {
        Debug.Log("Checkpoint1!");
        parent = GetComponentInParent<Enemy>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Finding Type parent!");
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Sending Info to parent!");
            parent.OnDetectionTriggerEnter(other);
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Finding Type parent!");
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Sending Info to parent!");
            parent.OnDetectionTriggerExit(other);
        }
    }
}

