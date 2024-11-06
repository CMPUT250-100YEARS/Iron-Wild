using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : AnimatedEntity
{

    public Transform player;

    [Header("Hover Settings")]
    private float hoverSpeed = 2f;
    private float diveSpeed = 10f;
    private float hoverHeight = 4f; // y-offset
    private float xOffset = 6f;

    private float hoverTimer;
    private float hoverDuration = 15f;

    public bool isHovering;
    public bool canAttack;
    private bool isHoveringToPos1;


    [Header("Warning Settings")]
    private bool playerInWarningCircle;
    public bool hasBeenWarned;

    private Vector3 warningCircleCenter;

    private float pulseSpeed = .1f; // Speed of the pulsing effect
    private float maxScale = .4f; // Maximum scale of the circle
    private float minScale = .3f;   // Minimum scale of the circle

    public GameObject warningCirclePrefab; // Assign a prefab with a SpriteRenderer (circle sprite)
    private GameObject warningCircleInstance;

    private float warningTimer;
    private float warningDuration = 4f;


    [Header("Animation Settings")]
    //public Sprite idleUp, idleRight, idleDown, idleLeft;
    public List<Sprite> upWalkCycle, rightWalkCycle, downWalkCycle, leftWalkCycle;
    private Vector3 _priorPosition;
    private int _direction = -1;//0 is up, 1 is right, 2 is down, 3 is left
    private float minDiff = 0.00001f;

    private List<Sprite> currentSpriteCycle;


    [Header("Miniboss Settings")]
    public float minibossHealth = 400f; 



    private void Start()
    {
        canAttack = false;
        isHovering = true;
        hasBeenWarned = false;
        isHoveringToPos1 = true;
        hoverTimer = hoverDuration;

        warningCircleInstance = null;
        AnimationSetup();
        _priorPosition = transform.position;

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
                    FindObjectOfType<Heart>().TakeDamage();
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

        //Animation Update based on movement
        if ((transform.position.y - _priorPosition.y) > minDiff)
        {
            //Moving Up
            if (_direction != 0)
            {
                _direction = 0;
                currentSpriteCycle = upWalkCycle;
            }
        }
        if ((_priorPosition.y - transform.position.y) > minDiff)
        {
            //Moving Down
            if (_direction != 2)
            {
                _direction = 2;
                currentSpriteCycle = downWalkCycle;
            }
        }

        if ((transform.position.x - _priorPosition.x) > minDiff)
        {
            //Moving right
            if (_direction != 1)
            {
                _direction = 1;
                currentSpriteCycle = rightWalkCycle;
            }
        }
        if ((_priorPosition.x - transform.position.x) > minDiff)
        {
            //Moving left
            if (_direction != 3)
            {
                _direction = 3;
                currentSpriteCycle = leftWalkCycle;
            }
        }

        //Animation Handling!
        if ((_priorPosition - transform.position).magnitude > minDiff)
        {
            
            AnimationUpdate();//Animate if moving
            SetCurrentAnimationCycle(currentSpriteCycle);

        }

        // this is for idle animation - probably won't need it.
        //else
        //{//Pick idle sprite if not moving
        //    if (_direction == 0)
        //    {
        //        SpriteRenderer.sprite = idleUp;
        //    }
        //    else if (_direction == 1)
        //    {
        //        SpriteRenderer.sprite = idleRight;
        //    }
        //    else if (_direction == 2)
        //    {
        //        SpriteRenderer.sprite = idleDown;
        //    }
        //    else if (_direction == 3)
        //    {
        //        SpriteRenderer.sprite = idleLeft;
        //    }

        //}

        //Grab the priorPosition
        _priorPosition = transform.position;



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

    public void takeDamage(float damage)
    {
        minibossHealth -= damage;
        if (minibossHealth <= 0) Destroy(gameObject);
    }



}