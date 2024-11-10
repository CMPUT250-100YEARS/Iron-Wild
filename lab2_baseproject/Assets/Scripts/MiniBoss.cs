using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : AnimatedEntity
{

    public Transform player;
    public AudioSource audioSource;

    [Header("Hover Settings")]
    private float hoverSpeed = 3f;
    private float diveSpeed = 12f;
    private float hoverHeight = 4f; // y-offset
    private float xOffset = 6f;

    private float hoverTimer;
    private float hoverDuration = 5.5f;

    public bool isHovering;
    public bool canAttack;
    private bool isHoveringToPos1;
    private bool isAlive = true;

    //Sound Effects
    public AudioClip clunk1;
    public AudioClip clunk2;
    public AudioClip clunk3;
    public AudioClip deadblast;
    public AudioClip bird1;
    public AudioClip bird2;
    public AudioClip bird3;
    public AudioClip bird4;

    private bool makesound = true;
    private bool hitsound = true;


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
    private float warningDuration = 0.7f;


    [Header("Animation Settings")]
    //public Sprite idleUp, idleRight, idleDown, idleLeft;
    public List<Sprite> upWalkCycle, rightWalkCycle, downWalkCycle, leftWalkCycle;
    private Vector3 _priorPosition;
    private int _direction = -1;//0 is up, 1 is right, 2 is down, 3 is left
    private float minDiff = 0.00001f;

    private List<Sprite> currentSpriteCycle;

    public Material flashMaterial; //Colour for the enemy to flash when taking damage
    private Material originalMaterial;
    private SpriteRenderer flashSpriteRenderer;
    private Coroutine flashroutine;


    [Header("Miniboss Settings")]
    public float minibossHealth; 



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

        //Set up the sprite renderer for flash effects
        flashSpriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = flashSpriteRenderer.material;

    }

    private void Update()
    {
        if (isAlive)
        {
            if (canAttack && hasBeenWarned)
            {
                StartCoroutine(Attacksound());

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
        EnemyFlash();
        if (minibossHealth <= 0 && isAlive) StartCoroutine(Death());
        else
        {
            int random_sound = Random.Range(1, 4);
            if (random_sound == 1) audioSource.PlayOneShot(clunk3);
            else if (random_sound == 2) audioSource.PlayOneShot(clunk2);
            else audioSource.PlayOneShot(clunk1);
            
            StartCoroutine(BirdHitsound());
        }
    }

    private IEnumerator Death()
    {
        isAlive = false;
        for (int i = 0; i < 6; i++)
        {
            audioSource.PlayOneShot(deadblast);
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private IEnumerator Attacksound()
    {
        if (makesound)
        {
            int random_sound = Random.Range(1, 3); //play attack sound
            if (random_sound == 1) audioSource.PlayOneShot(bird3);
            else audioSource.PlayOneShot(bird4);

            makesound = false;
            yield return new WaitForSeconds(2f);
            makesound = true;

        }
    }

    private IEnumerator BirdHitsound()
    {
        if (hitsound && makesound)
        {
            int birdnoise = Random.Range(1, 12);
            if (birdnoise == 1) audioSource.PlayOneShot(bird1);
            else if (birdnoise == 2) audioSource.PlayOneShot(bird2);
            
            hitsound = false;
            yield return new WaitForSeconds(3f);
            hitsound = true;
        }
    }

    public void EnemyFlash()
    {
        //Call the flash, if it is not currently playing
        if (flashroutine != null)
        {
            StopCoroutine(flashroutine);
        }

        flashroutine = StartCoroutine(hitFlash());
    }

    public IEnumerator hitFlash()
    {
        //Flash when taking damage
        flashSpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.12f);
        flashSpriteRenderer.material = originalMaterial;
        flashroutine = null;
    }



}