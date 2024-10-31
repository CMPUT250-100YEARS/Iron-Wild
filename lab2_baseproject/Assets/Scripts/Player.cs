using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct CinematicStep
{
    public Vector3 location;
    public string statement;
    public float timeAtLocation;
}

public class Player : AnimatedEntity
{

    public float Speed = 5;  // 5
    private AudioSource audioSource;

    public List<Sprite> InterruptedCycle;

    // footprint's init ----------------------------------------------------------
    public GameObject footprintPrefab;
    private float footprintDelay = 0.5f;
    private float footprintTimer = 0.0f;

    // -------------------------------------------------------------------------

    public GameObject bulletPrefab;
    private float fireRate = 0.4f;
    public float cooldown;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody2D rb;
    public LayerMask SolidObjectsLayer; //the foreground

    public float foodCount = 0;
    public string currScene;
    public string currDialog;
    public Vector3 startPosition;

    bool isFacingLeft;
    private SpriteRenderer gunSpriteRenderer;
    Transform weaponend;
    bool PrevShootRight = false;
    bool PrevShootLeft = false;

    private bool hasAbility_Dash;
    private float dashSpeed;

    //public float dashSpeed = 20f; // Speed of the dash
    private float dashDuration = 0.1f; // Duration of the dash
    private bool isDashing = false;
    private float dashTime;

    private Vector2 dashDirection;

    public Material flashMaterial; //Colour for the player to flash when taking damage
    private Material originalMaterial;
    private SpriteRenderer flashSpriteRenderer;
    private Coroutine flashroutine;

    // for sound in FOOD AND WATER
    //public AudioClip foodSound;
    public AudioClip waterSound;
    //public AudioSource playerDead;
    public AudioClip shootSound;

    //Sprite list based on mouse direction
    public List<Sprite> BackSpriteList;
    public List<Sprite> RightSpriteList;
    public List<Sprite> LeftSpriteList;
    public List<Sprite> FrontSpritList;

    //IDLE SPRITE*********************************************
    public List<Sprite> IdleBackSprite;
    public List<Sprite> IdleRightSprite;
    public List<Sprite> IdleLeftSprite;
    public List<Sprite> IdleFrontSprite;


    private List<Sprite> currentSpriteCycle;
    private Animator shootAnimator;
    private Transform aimTransform;

    public bool endDialogue;

    //cutscene stuff
    public bool cinematicControlled = true;
    public List<CinematicStep> cinematicSteps;
    private int _cinematicIndex;
    public GameObject uiCanvas;
    public Text text;
    private float _cutsceneTimer = 0;

    // START ADDED
    public float cutsceneSpeed = 2f;
    //public Sprite idleUp, idleRight, idleDown, idleLeft;
    //public List<Sprite> upWalkCycle, rightWalkCycle, downWalkCycle, leftWalkCycle;
    private Vector3 _priorPosition;
    private int _direction = -1;//0 is up, 1 is right, 2 is down, 3 is left
    private float minDiff = 0.00001f;
    // END ADDED

    public bool tutorialPuddle = false;
    public bool tutorialFood = false;
    public bool tutorialMutation = false;
    public string text_message;
    public bool showText = true;


    void Start()
    {
        AnimationSetup();
        _priorPosition = transform.position;  // ADDED
        audioSource = gameObject.GetComponent<AudioSource>();

        aimTransform = transform.Find("Aim");
        weaponend = aimTransform.Find("weaponend");
        //Transform weaponend = aimTransform.Find("weaponend");
        shootAnimator = weaponend.GetComponent<Animator>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        hasAbility_Dash = false;
        dashSpeed = Speed * 4;

        foodCount = 0;


        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "SampleScene")
        {
            PlayerPrefs.SetInt("numHearts", 6);  // set num hearts initially 
            PlayerPrefs.Save();
            Debug.Log("#2hearts" + PlayerPrefs.GetInt("numHearts"));
        }
        else
        {
            Debug.Log("#6hearts" + PlayerPrefs.GetInt("numHearts"));
        }

        Heart heartScript = FindObjectOfType<Heart>();
        heartScript.InitializeHearts();
        FindObjectOfType<Heart>().UpdateHearts();

        uiCanvas.SetActive(true);
        text_message = "Move around using the arrow keys";
        StartCoroutine(AnimateSpeech(text_message));
        StartCoroutine(Pause(5f));

        //Set up the sprite renderer for flash effects
        flashSpriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = flashSpriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();

        //Movement controls |start with player not moving
        bool isMoving = false;
        Vector3 inputDirection = Vector3.zero;

        //Angle between the player and cursor to determine SpriteList
        mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = mousePointer - transform.position;
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        //Make angle between 0 and 360 degrees
        if (angle < 0) { angle += 360f; }

        //******************************************8
        AimGun(angle);

        //What to do if the player is being controlled by a cinemtic
        if (cinematicControlled)
        {
            if (_cinematicIndex < cinematicSteps.Count)
            {
                //Move player to first cinematicSteps location if not there yet
                if ((transform.position - cinematicSteps[_cinematicIndex].location).magnitude > 0.005f)
                {
                    transform.position += (cinematicSteps[_cinematicIndex].location - transform.position).normalized * Time.deltaTime * Speed;
                }
                else
                {
                    //Set player location to avoid float issues
                    transform.position = cinematicSteps[_cinematicIndex].location;
                    if (_cutsceneTimer >= cinematicSteps[_cinematicIndex].timeAtLocation)
                    {
                        _cinematicIndex += 1;//Move on to next step if there is one
                        _cutsceneTimer = 0;
                        uiCanvas.SetActive(false);
                    }
                    else
                    {
                        //Display text during timer if there is any
                        if (cinematicSteps[_cinematicIndex].statement != "")
                        {
                            uiCanvas.SetActive(true);
                            text.text = cinematicSteps[_cinematicIndex].statement;
                        }

                        _cutsceneTimer += Time.deltaTime;
                    }
                }

                if ((transform.position.y - _priorPosition.y) > minDiff)
                {
                    //Moving Up
                    if (_direction != 0)
                    {
                        _direction = 0;
                        currentSpriteCycle = BackSpriteList;
                    }
                }
                if ((_priorPosition.y - transform.position.y) > minDiff)
                {
                    //Moving Down
                    if (_direction != 2)
                    {
                        _direction = 2;
                        currentSpriteCycle = FrontSpritList;
                    }
                }

                if ((transform.position.x - _priorPosition.x) > minDiff)
                {
                    //Moving right
                    if (_direction != 1)
                    {
                        _direction = 1;
                        currentSpriteCycle = RightSpriteList;
                    }
                }
                if ((_priorPosition.x - transform.position.x) > minDiff)
                {
                    //Moving left
                    if (_direction != 3)
                    {
                        _direction = 3;
                        currentSpriteCycle = LeftSpriteList;
                    }
                }


                // animation handling
                if ((_priorPosition - transform.position).magnitude > minDiff)
                {
                    AnimationUpdate();
                }
                else
                {
                    if (_direction == 0)
                    {
                        currentSpriteCycle = IdleBackSprite;
                    }
                    else if (_direction == 1)
                    {
                        currentSpriteCycle = IdleRightSprite;
                    }
                    else if (_direction == 2)
                    {
                        currentSpriteCycle = IdleFrontSprite;
                    }
                    else if (_direction == 3)
                    {
                        currentSpriteCycle = IdleLeftSprite;
                    }
                }
            }
            else
            {
                // Return control to the player at the end of this
                cinematicControlled = false;
            }
        }
        else
        {
            // movemenet if not cinematic controlled
            if (!endDialogue)
            {
                isMoving = false;
                //check input WASD and store direction
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    //Debug.Log("Player Update up-arrow" + PlayerPrefs.GetInt("numHearts"));
                    //transform.position+= Vector3.up*Time.deltaTime*Speed;
                    inputDirection += Vector3.up;
                    isMoving = true;
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    //transform.position+= Vector3.left*Time.deltaTime*Speed;
                    inputDirection += Vector3.left;
                    isMoving = true; //checking          
                }

                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    //transform.position+= Vector3.down*Time.deltaTime*Speed;
                    inputDirection += Vector3.down;
                    isMoving = true;
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    //transform.position+= Vector3.right*Time.deltaTime*Speed;
                    inputDirection += Vector3.right;
                    isMoving = true;
                }

                //If isMoving ==true, check for collision(foreground) then move
                if (isMoving)
                {
                    //Debug.Log("Player Update isMoving" + PlayerPrefs.GetInt("numHearts"));
                    Vector3 targetPos = transform.position + (inputDirection.normalized * Time.deltaTime * Speed);
                    //Vector3 targerPos = rb.position + inputDirection * Time.deltaTime * Speed; //inputDirection Vector2.up/down/...

                    // finding the footprint rotation on z-axis only --------------------------------------------------------------------
                    // 1. Calculate the angle in the XY plane (z-axis rotation) relative to the world up vector
                    // 2. Apply only the Z rotation, keeping x and y as they are

                    float targetAngle = Mathf.Atan2(inputDirection.normalized.y, inputDirection.normalized.x) * Mathf.Rad2Deg;
                    Quaternion footRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetAngle);

                    // ------------------------------------------------------------------------------------------------------------------
                    if (!IsCollidingWith(targetPos))
                    {

                        if (isDashing)
                        {
                            //Debug.Log("isDashing" + isDashing);
                            //Debug.Log("HEREEEEE!!! speed increased to True");
                            transform.position += inputDirection.normalized * Time.deltaTime * dashSpeed;
                        }
                        else
                        {
                            transform.position += inputDirection.normalized * Time.deltaTime * Speed;

                            // code to implement footprints; delay added to prevent update on every frame. --------------------------------------
                            if (footprintTimer >= footprintDelay){
                                Instantiate(footprintPrefab, transform.position + new Vector3(0, 0, -1), footRotation);
                                footprintTimer = 0.0f;
                            }
                            else{
                                footprintTimer += Time.deltaTime;
                            }
                            // ------------------------------------------------------------------------------------------------------------------

                        }
                    }
                }

                //Determine SpriteList based on angle
                if (angle > 45f && angle <= 135f)
                {
                    currentSpriteCycle = BackSpriteList; //when Moving
                    if (!isMoving) { currentSpriteCycle = IdleBackSprite; } //***************when not moving
                }

                else if (angle > 135f && angle <= 225f)
                {
                    currentSpriteCycle = LeftSpriteList;
                    if (!isMoving) { currentSpriteCycle = IdleLeftSprite; } //***************when not moving
                }

                else if (angle > 225f && angle <= 315f)
                {
                    currentSpriteCycle = FrontSpritList;
                    if (!isMoving) { currentSpriteCycle = IdleFrontSprite; } //***************when not moving
                }

                else if (angle > 315f || angle <= 45f)
                {
                    currentSpriteCycle = RightSpriteList;
                    if (!isMoving) { currentSpriteCycle = IdleRightSprite; } //***************when not moving
                }
                //call SetMovementDirection and SetCurrentAnimationCycle
                SetMovementDirection(isMoving);
                SetCurrentAnimationCycle(currentSpriteCycle);
                //AnimationUpdate();
            }
        }

        //Grab the priorPosition
        _priorPosition = transform.position;

        //call SetMovementDirection and SetCurrentAnimationCycle
        SetMovementDirection(isMoving);
        SetCurrentAnimationCycle(currentSpriteCycle);




        //shooting

        if (Input.GetKey(KeyCode.Mouse0) && cooldown <= 0.0f)
        {
            cooldown = fireRate;
            Shoot();
        }
        else
        {
            if (cooldown >= 0.0f)
            {
                cooldown -= Time.deltaTime;
            }
        }


        //if (hasAbility_Dash && Input.GetKeyDown(KeyCode.Space))
        //{
        //    Dash();
        //}
        if (!isDashing && Input.GetKeyDown(KeyCode.Space) && hasAbility_Dash)
        {
            isDashing = true;
            dashTime = dashDuration;
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
            }
        }

        if (!showText)  // tutorial text
        {
            uiCanvas.SetActive(false);
        }
    }

    void AimGun(float angle)
    {
        Transform aimtransform = transform.Find("Aim");
        if (aimtransform != null)
        {
            mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mousePointer - transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            aimtransform.rotation = Quaternion.Euler(0, 0, rotZ);

            float distanceFromPlayer = 1f;
            Vector3 gunPosition = new Vector3(Mathf.Cos(rotZ * Mathf.Deg2Rad), Mathf.Sin(rotZ * Mathf.Deg2Rad), 0) * distanceFromPlayer;

            aimtransform.localPosition = gunPosition;

            isFacingLeft = mousePointer.x < transform.position.x;

            Transform gunTransform = transform.Find("Aim/Gun");  // access gun

            if (gunTransform != null)
            {
                gunSpriteRenderer = gunTransform.GetComponent<SpriteRenderer>();
            }

            // flip gun image depending where mouse cursor is
            if (gunSpriteRenderer != null)
            {
                if (isFacingLeft)
                {
                    gunSpriteRenderer.flipY = true;
                }
                else
                {
                    gunSpriteRenderer.flipY = false;
                }
            }

            //if (angle <= 90f || angle >= 270f)
            //{
            //    aimtransform.localPosition = new Vector3(1f, 1f, 0);
            //}
            //else
            //{
            //    aimtransform.localPosition = new Vector3(-1f, 0, 0);
            //}
        }
    }

    bool IsCollidingWith(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, SolidObjectsLayer) != null)
        {
            return true; //player colliding with an object in foreground
        }
        return false; //no collision - player can move
    }

    public IEnumerator Pause(float time)
    {
        yield return new WaitForSeconds(time);
        text_message = "I need to find water so my water level doesn't run out";
        StartCoroutine(AnimateSpeech(text_message));
    }

    public IEnumerator Pause2(float time)
    {
        yield return new WaitForSeconds(time);
        showText = false;
    }

    public IEnumerator LevelChangeWait(float time)
    {
        yield return new WaitForSeconds(time);


        currScene = SceneManager.GetActiveScene().name;

        if (currScene == "SampleScene")
        {
            Debug.Log("LevelChangeWait Load CITY scene");
            SceneManager.LoadScene("CITY");
        }
        else // if (currScene == "CITY")
        {
            Debug.Log("LevelChangeWait Load RoofTop scene");
            SceneManager.LoadScene("RoofTop");
        }

        //Time.timeScale = 1f;
        endDialogue = false;
    }


    public IEnumerator AnimateSpeech(string message)
    {
        uiCanvas.SetActive(true);
        text.text = "";
        //speechBubble.transform.position += new Vector3(50f, 50f, 0);
        //uiCanvas.transform.position += new Vector3(100f, 100f, 0);

        foreach (char letter in message)
        {
            text.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        //yield return new WaitForSeconds(2f);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player collider entered with: " + other.gameObject.name);

        Pickup pickup = other.gameObject.GetComponent<Pickup>();
        if (pickup != null)
        {

            hasAbility_Dash = true;
            Interrupt(InterruptedCycle);
            if (audioSource != null)
            {
                audioSource.Play();
            }

            if (!tutorialMutation)
            {
                tutorialMutation = true;
                showText = true;
                //Debug.Log("found tutorial puddle!");
                //uiCanvas.SetActive(true);
                text_message = "Watch out for enemies. Aim and shoot with your mouse. Be careful, otherwise you may lose a heart!";
                StartCoroutine(AnimateSpeech(text_message));
                StartCoroutine(Pause2(10f));
            }
            //pickup.Reset();
        }



        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {

            //if (playerDead != null)
            //{
            //    playerDead.Play();
            //}

            GameOverManager gameOver = FindObjectOfType<GameOverManager>();
            FindObjectOfType<Heart>().TakeDamage();
            PlayerFlash();
        }




        PuddleScript puddle = other.gameObject.GetComponent<PuddleScript>();
        if (puddle != null)
        {

            Debug.Log("found puddle!");
            FindObjectOfType<WaterManager>().IncreaseWater(20f); //???
            Destroy(puddle.gameObject);

            if ((waterSound != null) && (audioSource != null))
            {
                audioSource.PlayOneShot(waterSound);
            }

            if (!tutorialPuddle)
            {
                tutorialPuddle = true;
                showText = true;
                //Debug.Log("found tutorial puddle!");
                //uiCanvas.SetActive(true);
                text_message = "Now it's time to find food";
                StartCoroutine(AnimateSpeech(text_message));
            }
        }


        FoodObject food = other.gameObject.GetComponent<FoodObject>();

        if (food != null)
        {
            foodCount++;
            FindObjectOfType<FoodImage>().FoundFoods();
            Destroy(food.gameObject);

            if (!tutorialFood)
            {
                tutorialFood = true;
                showText = true;
                //Debug.Log("found tutorial puddle!");
                //uiCanvas.SetActive(true);
                text_message = "Mutations can make you run faster";
                StartCoroutine(AnimateSpeech(text_message));
            }
        }

        LevelEndTrigger levelEnd = other.gameObject.GetComponent<LevelEndTrigger>();

        if (levelEnd != null)
        {
            Vector3 playerPosition = this.transform.position;

            //if (foodCount < 2)
            //{
            //    FindObjectOfType<LevelEndTrigger>().OnLevelComplete("I need more food!");
            //} else
            //{
            endDialogue = true;



            currScene = SceneManager.GetActiveScene().name;

            if (currScene == "SampleScene")
            {
                currDialog = "ONTO THE CITY";
            }
            else // if (currScene == "CITY")
            {
                currDialog = "ONTO THE ROOFTOP";
            }
            FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog);

            StartCoroutine(LevelChangeWait(3f));
            //SceneManager.LoadScene("CITY");

            //Heart heartScript = FindObjectOfType<Heart>();
            //heartScript.InitializeHearts();
            //PlayerPrefs.SetInt("numHearts", numHearts);
            //PlayerPrefs.Save();
            //}
            //FindObjectOfType<LevelEndTrigger>().ShowSpeechBubble(foodCount);

        }

    }

    void Shoot()
    {
        if (aimTransform != null)
        {
            GameObject aimObject = aimTransform.gameObject;

            mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mousePointer - transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            aimTransform.rotation = Quaternion.Euler(0, 0, rotZ);
            Transform guntransform = aimTransform.Find("Gun");

            Vector3 guntransformGun;
            Vector3 bulletOffset = new Vector3(0, 0.2f, 0);     // Align bullets with gun
            Vector3 weaponEndOffset = new Vector3(0, -0.3f, 0); // Align weapon end with gun

            if (guntransform != null)
            {
                GameObject gunObject = guntransform.gameObject;

                if (isFacingLeft)
                {
                    guntransformGun = guntransform.position + bulletOffset; // Position the stream of bullets
                    weaponend.localPosition += weaponEndOffset; // Position the weapon end
                    shootAnimator.SetTrigger("Shoot");
                    //GameObject gunObject = guntransform.gameObject;
                    //Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
                    Instantiate(bulletPrefab, guntransformGun, Quaternion.identity);

                    // TEMP COMMENTED OUT
                    //CineMCamShake.Instance.ShakeCamera(5f, .1f);
                    //audioSource.PlayOneShot(shootSound);

                    // If did not switch gun direction, then reset weapon end for next time
                    if (PrevShootLeft)
                    {
                        weaponend.localPosition -= weaponEndOffset;

                        //guntransformGun -= bulletOffset; //???oct 24 t e m p
                    }

                    PrevShootLeft = true;
                    PrevShootRight = false;
                }
                else // Player is facing right
                {
                    // If switched gun direction, then reposition weapon end
                    if (PrevShootLeft)
                    {
                        weaponend.localPosition -= weaponEndOffset;
                        shootAnimator.SetTrigger("Shoot");
                        Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);

                        PrevShootRight = true;
                        PrevShootLeft = false;

                    }
                    else
                    {
                        shootAnimator.SetTrigger("Shoot");
                        Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
                    }
                }

            }
            //GameObject.Instantiate(bulletPrefab, guntransform.position, guntransform.rotation, gunObject.transform);
        }
    }

    public float getSpeed()
    {
        return Speed;
    }

    public void PlayerFlash()
    {
        //Call the flash, if it is not currently playing
        if (flashroutine != null)
        {
            StopCoroutine(flashroutine);
        }

        flashroutine = StartCoroutine(damageFlash());
    }

    public IEnumerator damageFlash()
    {
        //Flash when taking damage
        flashSpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.12f);
        flashSpriteRenderer.material = originalMaterial;
        flashroutine = null;
    }

    //void Dash()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, Vector3.right, 5);
    //}

    //    private void StartDash()
    //    {
    //        isDashing = true;
    //        dashTime = dashDuration;

    //        // Get the direction the player is currently moving or facing
    //        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

    //        // If no input, default to dashing right
    //        if (dashDirection == Vector2.zero)
    //        {
    //            dashDirection = Vector2.right; // Default direction
    //        }

    //        // Calculate the potential target position after dash
    //        Vector2 targetPos = rb.position + dashDirection * dashSpeed * dashDuration;

    //        // Check if the player would collide with a boundary
    //        if (!IsCollidingWith(targetPos))
    //        {
    //            // Apply the dash velocity
    //            rb.velocity = dashDirection * dashSpeed;
    //        }
    //    }

    //    private void StopDash()
    //    {
    //        isDashing = false;
    //        rb.velocity = Vector2.zero; // Stops movement after the dash ends
    //    }

    public void Restart()
    {
        Debug.Log("Player Restart!"); //oct17
        // Reset player state
        foodCount = 0;
        //startPosition = transform.position;

        //PlayerPrefs.SetInt("numHearts", 2); // set num hearts initially
        //PlayerPrefs.Save();
    }


}