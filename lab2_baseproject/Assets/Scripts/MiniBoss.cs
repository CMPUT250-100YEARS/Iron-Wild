using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : MonoBehaviour
{

    public Transform player;

    private float hoverSpeed = 2f;
    private float diveSpeed = 10f;
    private float hoverHeight = 4f; // y-offset
    private float xOffset = 6f;

    private float hoverTimer;
    private float hoverDuration = 15f;

    private float warningTimer;
    private float warningDuration;

    private bool isHovering;
    private bool canAttack;
    private bool hasBeenWarned;

    private bool isHoveringToPos1;
    //private bool isHoveringToPos2;




    private void Start()
    {
        canAttack = false;
        isHovering = true;
        hasBeenWarned = false;
        isHoveringToPos1 = true;
        hoverTimer = hoverDuration;

    }

    private void Update()
    {
        
        if (canAttack && hasBeenWarned)
        {
            //attack
            transform.position += diveSpeed * Time.deltaTime * (player.position - transform.position).normalized;
            if ((player.position - transform.position).magnitude < 0.2f)
            {
                canAttack = false;
                hoverTimer = hoverDuration;
            }

        }
        
        else
        {
            //hover
            if (hoverTimer > 0f) //&& something
            {
                // hover
                
                Vector2 hoverPosition1 = new Vector2(player.position.x - xOffset, player.position.y + hoverHeight);
                Vector2 hoverPosition2 = new Vector2(player.position.x + xOffset, player.position.y + hoverHeight);

                if (isHoveringToPos1)
                {
                    transform.position = Vector2.MoveTowards(transform.position, hoverPosition1, hoverSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, hoverPosition1) < 0.1f)
                    {
                        isHoveringToPos1 = false;  // Start moving to hoverPosition2
                    }

                }
                else 
                {
                    transform.position = Vector2.MoveTowards(transform.position, hoverPosition2, hoverSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, hoverPosition2) < 0.1f)
                    {
                        isHoveringToPos1 = true;  // Start moving to hoverPosition1
                    }

                }
                hoverTimer -= Time.deltaTime;
            }
            else
            {
                canAttack = true;
                Warn();
            }
            
        }


    }

    //void DiveAttack()
    //{

    //}

    //void HoverAbove()
    //{

    //}

    void Warn()
    {
        Debug.Log("Warning to implement");
        hasBeenWarned = true;
    }
}