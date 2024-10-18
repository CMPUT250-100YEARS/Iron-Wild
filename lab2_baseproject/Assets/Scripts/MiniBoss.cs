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
    private float warningDuration = 4f;

    public bool isHovering;
    public bool canAttack;
    public bool hasBeenWarned;

    private bool isHoveringToPos1;
    private bool playerInWarningCircle;

    private Vector3 warningCircleCenter;

    private float pulseSpeed = .1f; // Speed of the pulsing effect
    private float maxScale = .4f; // Maximum scale of the circle
    private float minScale = .3f;   // Minimum scale of the circle

    public GameObject warningCirclePrefab; // Assign a prefab with a SpriteRenderer (circle sprite)
    private GameObject warningCircleInstance;

    private void Start()
    {
        canAttack = false;
        isHovering = true;
        hasBeenWarned = false;
        isHoveringToPos1 = true;
        hoverTimer = hoverDuration;

        warningCircleInstance = null;


    }

    private void Update()
    {
        
        if (canAttack && hasBeenWarned)
        {
            if (CheckIfPlayerInCircle())
            {
                transform.position += diveSpeed * Time.deltaTime * (player.position - transform.position).normalized;
                if ((player.position - transform.position).magnitude < 0.2f)
                {
                    Destroy(warningCircleInstance);
                    canAttack = false;
                    hoverTimer = hoverDuration;
                    isHovering = true;
                }
            }
            else
            {
                transform.position += diveSpeed * Time.deltaTime * (warningCircleCenter - transform.position).normalized;
                if ((warningCircleCenter - transform.position).magnitude < 0.2f)
                {
                    Destroy(warningCircleInstance);
                    canAttack = false;
                    hoverTimer = hoverDuration;
                    isHovering = true;
                }
            }
            //attack
            
            
            

        }

        if (isHovering)
        {
            if (hoverTimer > 0f) 
            {
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
                if (hoverTimer < warningDuration + 1f)
                {
                    Debug.Log("Call to ShowWarningCircle");
                    ShowWarningCircle();
                }
            }
            else
            {
                canAttack = true;
            }

            
        }

        if (warningCircleInstance != null)
        {
            warningTimer += Time.deltaTime;

            // Pulsating effect
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale) + minScale;
            warningCircleInstance.transform.localScale = new Vector3(scale, scale, 1f);

            // Destroy the circle after the warning duration
            if (warningTimer >= warningDuration)
            {
                hasBeenWarned = true;
                warningTimer = 0f;
            }
        }



    }

    void ShowWarningCircle()
    {
        // Spawn the warning circle at the player's position
        if (warningCircleInstance == null)
        {
            warningCircleInstance = Instantiate(warningCirclePrefab, player.position, Quaternion.identity);
            warningCircleCenter = warningCircleInstance.transform.position;
        }

    }

    bool CheckIfPlayerInCircle()
    {
        // Calculate the distance between the player and the warning circle center
        float distanceToPlayer = Vector2.Distance(player.position, warningCircleCenter);
        SpriteRenderer spriteRenderer = warningCircleInstance.GetComponent<SpriteRenderer>();
        //float warningRadius = spriteRenderer.bounds.extents.x;
        float warningRadius = 9.5f;
        // Compare the distance to the circle's radius (considering the maxScale as the circle's size)
        float actualWarningRadius = warningRadius * maxScale;

        if (distanceToPlayer <= actualWarningRadius)
        {
            
            Debug.Log("Player is inside the warning circle!");
            // Apply damage or any other effect
            return true;
        }
        else
        {
            
            Debug.Log("Player is outside the warning circle.");
            return false;
        }
    }




}