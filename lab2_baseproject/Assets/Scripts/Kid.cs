using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kid : MonoBehaviour
{

    private Player fishmom;
    private Transform fishmom_transform; // Reference to the parent's Transform

    public float followSpeed = 2f;
    public float followDistance = 1.5f;


    public float runAwaySpeed = 4f;
    public float runAwayTime = 2f; // How long the kid runs away

    private bool isRunningAway = false;
    private float runAwayTimer = 0f;


    private SpriteRenderer spriteRenderer;
    


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();


        fishmom = FindObjectOfType<Player>();
        fishmom_transform = fishmom.transform;
        followSpeed = fishmom.getSpeed() * 0.8f;

        runAwaySpeed = fishmom.getSpeed() * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToParent = Vector2.Distance(transform.position, fishmom_transform.position);

        if (spriteRenderer.enabled)
        {
            if (!isRunningAway)
            {
                if (distanceToParent > followDistance)
                {
                    Vector2 direction = (fishmom_transform.position - transform.position).normalized;
                    transform.position = Vector2.MoveTowards(transform.position, fishmom_transform.position, followSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (runAwayTimer < runAwayTime) { RunAway(); runAwayTimer += Time.deltaTime; }
                else
                {
                    runAwayTimer = 0;
                    isRunningAway = false;
                    Disappear();
                }

            }


            if (Input.GetKey(KeyCode.R))
            {
                isRunningAway = true;
            }

            
        }
        if (Input.GetKey(KeyCode.T))
        {
            Reappear();
        }

    }


    // trigger when the kid runs away;
    void RunAway()
    {
        Debug.Log("Kid's running away");
      
        Vector2 direction = (transform.position - fishmom_transform.position).normalized;
        direction += (Random.insideUnitCircle * 0.5f);
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, runAwaySpeed * Time.deltaTime);

        
    }


    void Reappear()
    {
        spriteRenderer.enabled = true;
    }

    // disappear when out of camera view
    void Disappear()
    {
        spriteRenderer.enabled = false;
    }

}


//// make the kid comeback;
//void Comeback() { }





//// reappear outside the caamera view
//


