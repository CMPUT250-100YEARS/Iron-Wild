using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    public float fireRate = 0.4f;
    private float cooldown;

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
    
    bool PrevShootRight = false;
    bool PrevShootLeft = false;

    private bool hasAbility_Dash;
    private float dashSpeed;

    //public float dashSpeed = 20f; // Speed of the dash
    private float dashDuration = 0.13f; // Duration of the dash
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTime;
    public float dashCooldown;

    private Vector2 dashDirection;

    private Transform squishTransform;
    private float originalHeight;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    public Material flashMaterial; //Colour for the player to flash when taking damage
    private Material originalMaterial;
    private SpriteRenderer flashSpriteRenderer;
    private Coroutine flashroutine;

    // for sound
    public AudioClip foodSound;
    public AudioClip waterSound;
    public AudioClip waterSound2;
    public AudioClip shootSound1;
    public AudioClip shootSound2;
    public AudioClip dashSound1;
    public AudioClip dashSound2;
    public AudioClip dashSound3;
    public AudioClip dashSound4;
    public AudioClip dashSound5;
    public AudioClip dashrecharged;
    public AudioClip damageSound;
    public AudioClip interactSound;
    public AudioClip walkSound1;
    public AudioClip walkSound2;
    public AudioClip walkSound3;
    public AudioClip walkSound4;
    public AudioClip walkSound5;
    public AudioClip walkSound6;
    public AudioClip walkSound7;
    public AudioClip walkSound8;

    //Used with random sounds
    private int dashsoundnum;
    private int walksoundnum;
    private int walkstep = 1;
    private int walksoundtimer = 1;

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
    private Transform gunTransform;
    private Transform weaponend;

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
    private bool isMoving;
    // END ADDED

    public static bool gunMoveable = true;

    // tutorial variables
    public bool tutorialPuddle = false;
    public bool tutorialFood = false;
    public bool tutorialMutation = false;
    public string text_message;
    public bool showText = true;
    public bool movementTextDone = false;
    public bool waterTextDone = false;
    public bool foodTextDone = false;
    public bool mutationsTextDone = false;
    public bool enemyTextDone = false;
    public bool tutorialFinished = false;
    public Image tutorialImage;
    public Image waterImage;
    public Image foodImage;
    public Image enemyImage;
    public Image mutationImage;
    public bool waterStart = false;
    public bool foodStart = false;
    public bool mutationsStart = false;
    public bool enemyStart = false;


    void Start()
    {
        AnimationSetup();
        _priorPosition = transform.position;  // ADDED
        audioSource = gameObject.GetComponent<AudioSource>();

        aimTransform = transform.Find("Aim");
        gunTransform = aimTransform.Find("Gun");
        weaponend = gunTransform.Find("weaponend");

        if (weaponend is null)
        {
            Debug.Log("weaponend null!");
        }

        //Transform weaponend = aimTransform.Find("weaponend");
        shootAnimator = weaponend.GetComponent<Animator>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        hasAbility_Dash = true;
        dashSpeed = Speed * 4;

        squishTransform = transform;
        originalHeight = squishTransform.localScale.y;
        originalScale = squishTransform.localScale;

        foodCount = 0;
        gunMoveable = true;


        Scene currentScene = SceneManager.GetActiveScene();
        //if (currentScene.name == "SampleScene")
        //{
        PlayerPrefs.SetInt("numHearts", 6);  // set num hearts initially 
        PlayerPrefs.Save();
        Debug.Log("#2hearts" + PlayerPrefs.GetInt("numHearts"));
        //}
        //else
        //{
        //    Debug.Log("#6hearts" + PlayerPrefs.GetInt("numHearts"));
        //}

        Heart heartScript = FindObjectOfType<Heart>();
        heartScript.InitializeHearts();
        FindObjectOfType<Heart>().UpdateHearts();

        uiCanvas.SetActive(true);
        text_message = "Move around using the arrow keys or WASD keys.                                        ";
        StartCoroutine(AnimateSpeech(text_message, "movement"));
        StartCoroutine(WalkSounds());

        //Set up the sprite renderer for flash effects
        flashSpriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = flashSpriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        //Changing the sort layer
        //UpdateSortingOrder();

        AnimationUpdate();

        //Movement controls |start with player not moving
        isMoving = false;
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
                    Vector3 dashPos = transform.position + (inputDirection.normalized * Time.deltaTime * dashSpeed);

                    // finding the footprint rotation on z-axis only --------------------------------------------------------------------
                    // 1. Calculate the angle in the XY plane (z-axis rotation) relative to the world up vector
                    // 2. Apply only the Z rotation, keeping x and y as they are

                    float targetAngle = Mathf.Atan2(inputDirection.normalized.y, inputDirection.normalized.x) * Mathf.Rad2Deg;
                    Quaternion footRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetAngle);
                    // ------------------------------------------------------------------------------------------------------------------


                    if (!IsCollidingWith(targetPos))
                    {

                        if ((isDashing) && (!IsCollidingWith(dashPos)))
                        {
                            transform.position += inputDirection.normalized * Time.deltaTime * dashSpeed;
                        }
                        else  // not dashing...just moving
                        {
                            transform.position += inputDirection.normalized * Time.deltaTime * Speed;
                            if (footprintTimer >= footprintDelay)
                            {
                                Instantiate(footprintPrefab, transform.position + new Vector3(0, -0.75f, -1), footRotation);
                                footprintTimer = 0.0f;
                            }
                            else
                            {
                                footprintTimer += Time.deltaTime;
                            }

                            // ------------------------------------------------------------------------------------------------------------------
                            // COMMENTED OUT CODE TO MAKE FOOT TRACKS ONLY CITY LEVEL.
                            //Scene currentScene = SceneManager.GetActiveScene();
                            //if (currentScene.name == "CITY")
                            //{
                            //transform.position += inputDirection.normalized * Time.deltaTime * Speed;
                            // code to implement footprints; delay added to prevent update on every frame. 
                            //}
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
            if (Random.Range(1, 4) == 1) audioSource.PlayOneShot(shootSound2);
            else audioSource.PlayOneShot(shootSound1);
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
        if (!isDashing && canDash && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && hasAbility_Dash)
        {
            isDashing = true;
            canDash = false;
            
            dashsoundnum = Random.Range(1, 6);
            if (dashsoundnum == 1) audioSource.PlayOneShot(dashSound1);
            else if (dashsoundnum == 2) audioSource.PlayOneShot(dashSound2);
            else if (dashsoundnum == 3) audioSource.PlayOneShot(dashSound3);
            else if (dashsoundnum == 4) audioSource.PlayOneShot(dashSound4);
            else audioSource.PlayOneShot(dashSound5);
            walksoundtimer = 0;

            dashTime = dashDuration;

            //if (!tutorialMutation && mutationsTextDone)
            //{
            //    tutorialMutation = true;
            //    showText = true;
            //    //Debug.Log("found tutorial puddle!");
            //    //uiCanvas.SetActive(true);
            //    text_message = "Robots are your enemy! Point with the mouse to aim and left click on it to shoot.";
            //    StartCoroutine(AnimateSpeech(text_message, "enemy"));
            //    StartCoroutine(Pause2(10f));
            //}
        }

        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                StartCoroutine(DashCooldown());
            }
        }

        if (!showText)  // tutorial text
        {
            uiCanvas.SetActive(false);
        }

        if (!tutorialFinished)
        {
           if (movementTextDone && waterTextDone && foodTextDone && mutationsTextDone && !enemyStart)
           {
                enemyStart = true;
                StartCoroutine(AnimateSpeech("Robots are your enemy! Point with the mouse to aim and left click on it to shoot.                                        ", "enemy"));
            } 
           else if (movementTextDone && waterTextDone && foodTextDone && !mutationsStart)
           {
                mutationsStart = true;
                StartCoroutine(AnimateSpeech("While you are walking, click shift key to speed up for a short amount of time to dodge incoming bullets.                                        ", "mutations"));
            }
           else if (movementTextDone && waterTextDone && !foodStart)
           {
                foodStart = true;
                StartCoroutine(AnimateSpeech("To survive in the wild, you will need food. Make sure you collect as much as you can to finish the level.                                        ", "food"));
            }
           else if (movementTextDone && !waterStart)
           {
                waterStart = true;
                StartCoroutine(AnimateSpeech("Your mutation makes it so you can't go long without water. Make sure your hydration meter doesn't run out by walking into water puddles.                                        ", "water"));
            }
        }
    }

    void AimGun(float angle)
    {
        Transform aimtransform = transform.Find("Aim");
        if ((aimtransform != null) && (gunMoveable))  // gun should only move if not paused or game over
        {
            Debug.Log("Gun Movable: " + gunMoveable);

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
        if (Physics2D.OverlapCircle(targetPos, 0.5f, SolidObjectsLayer) != null)
        {
            return true; //player colliding with an object in foreground
        }
        return false; //no collision - player can move
    }

    //public IEnumerator Pause(float time, string message, string objectType)
    //{
    //    yield return new WaitForSeconds(time);
    //    text_message = "Your mutation makes it so you can't go long without water. Make sure your hydration meter doesn't run out by walking into water puddles.";
    //    StartCoroutine(AnimateSpeech(message, "water"));
    //}

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
            //Debug.Log("LevelChangeWait Load CITY scene");
            //SceneManager.LoadScene("CITY"); //OLD
            SceneManager.LoadScene("Forest End");
        }
        else // if (currScene == "CITY")
        {
            Debug.Log("LevelChangeWait Load RoofTop scene");
            //SceneManager.LoadScene("RoofTop"); //OLD
            SceneManager.LoadScene("RoofTop");
        }

        //Time.timeScale = 1f;
        endDialogue = false;
    }


    public IEnumerator AnimateSpeech(string message, string objectType)
    {
        uiCanvas.SetActive(true);
        text.text = "";

        RectTransform rectTransform = tutorialImage.GetComponent<RectTransform>();  // for tutorial image
        // set images
        if (objectType == "water")
        {
            tutorialImage.color = Color.cyan;  // change colour filter
            //tutorialImage.color = new Color(10f, 231f, 212f, 1f);
            tutorialImage.sprite = waterImage.sprite;  // change image
            rectTransform.sizeDelta = new Vector2(55f, 30f);  // change width and height of image
        }
        else if (objectType == "food")
        {
            tutorialImage.color = Color.white;  // change colour filter
            tutorialImage.sprite = foodImage.sprite;
            rectTransform.sizeDelta = new Vector2(50f, 30f);
        }
        else if (objectType == "mutations")
        {
            tutorialImage.color = Color.gray;  // change colour filter
            tutorialImage.sprite = mutationImage.sprite;
            rectTransform.sizeDelta = new Vector2(45f, 25f);
        }
        else if (objectType == "enemy")
        {
            //tutorialImage.sprite = enemyImage.sprite;
            tutorialImage.color = Color.white;  // change colour filter
            tutorialImage.sprite = enemyImage.sprite;
            rectTransform.sizeDelta = new Vector2(50f, 60f);
        }

    //speechBubble.transform.position += new Vector3(50f, 50f, 0);
    //uiCanvas.transform.position += new Vector3(100f, 100f, 0);

    string newLetter;
        int charCount = 1;
        foreach (char letter in message)
        {
            if (objectType == "movement")  // 1-4, 23-27, 37-40
            {
                if (((charCount >= 1) && (charCount <= 4)) || ((charCount >= 23) && (charCount <= 27)) || ((charCount >= 37) && (charCount <= 40)))
                {
                    newLetter = "<b>" + letter + "</b>";
                    text.text += newLetter;
                }
                else
                {
                    text.text += letter;
                }
            }

            if (objectType == "water")  // 75-89 or 123-135
            {
                if (((charCount >= 75) && (charCount <= 89)) || ((charCount >= 123) && (charCount <= 135)))
                {
                    newLetter = "<b>" + letter + "</b>";
                    text.text += newLetter;
                }
                else
                {
                    text.text += letter;
                }
            }

            if (objectType == "food")  // 39 - 42 or 89 - 104
            {
                if (((charCount >= 39) && (charCount <= 42)) || ((charCount >= 89) && (charCount <= 104)))
                {
                    newLetter = "<b>" + letter + "</b>";
                    text.text += newLetter;
                }
                else
                {
                    text.text += letter;
                }
            }

            if (objectType == "mutations")  // 30-38, 43-50
            {
                if (((charCount >= 30) && (charCount <= 38)) || ((charCount >= 43) && (charCount <= 50)))
                {
                    newLetter = "<b>" + letter + "</b>";
                    text.text += newLetter;
                }
                else
                {
                    text.text += letter;
                }
            }

            if (objectType == "enemy")  // 17 - 21 or 39 - 43 or 56 - 65
            {
                if (((charCount >= 17) && (charCount <= 21)) || ((charCount >= 39) && (charCount <= 43)) || ((charCount >= 56) && (charCount <= 65)))
                {
                    newLetter = "<b>" + letter + "</b>";
                    text.text += newLetter;
                }
                else
                {
                    text.text += letter;
                }
            }

            charCount++;
            //text.text += letter;  // OLD WITHOUT BOLD
            yield return new WaitForSeconds(0.05f);
        }


        if (objectType == "movement")
        {
            movementTextDone = true;
        } else if (objectType == "water")
        {
            waterTextDone = true;
        } else if (objectType == "food")
        {
            foodTextDone = true;
        } else if (objectType == "mutations")
        {
            mutationsTextDone = true;
        } else if (objectType == "enemy")
        {
            enemyTextDone = true;
            tutorialFinished = true;
            uiCanvas.SetActive(false);
        }
        //yield return new WaitForSeconds(10f);  // commented out
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Player collider entered with: " + other.gameObject.name);

        Pickup pickup = other.gameObject.GetComponent<Pickup>();
        if (pickup != null)
        {

            hasAbility_Dash = true;
            Interrupt(InterruptedCycle);
            if (audioSource != null)
            {
                audioSource.PlayOneShot(interactSound);
            }

            //if (!tutorialMutation && mutationsTextDone)
            //{
            //    tutorialMutation = true;
            //    showText = true;
            //    //Debug.Log("found tutorial puddle!");
            //    //uiCanvas.SetActive(true);
            //    text_message = "Robots are your enemy! Point with the mouse to aim and left click on it to shoot.";
            //    StartCoroutine(AnimateSpeech(text_message, "enemy"));
            //    StartCoroutine(Pause2(10f));
            //}
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
                audioSource.PlayOneShot(waterSound2);
            }

            //if (!tutorialPuddle && waterTextDone)
            //{
            //    tutorialPuddle = true;
            //    showText = true;
            //    //Debug.Log("found tutorial puddle!");
            //    //uiCanvas.SetActive(true);
            //    text_message = "To survive in the wild, you will need food. Make sure you collect as much as you can to finish the level.";
            //    StartCoroutine(AnimateSpeech(text_message, "food"));
            //}
        }


        FoodObject food = other.gameObject.GetComponent<FoodObject>();

        if (food != null)
        {
            foodCount++;
            audioSource.PlayOneShot(foodSound);
            FindObjectOfType<FoodImage>().FoundFoods();
            Destroy(food.gameObject);

            //if (!tutorialFood && foodTextDone)
            //{
            //    tutorialFood = true;
            //    showText = true;
            //    //Debug.Log("found tutorial puddle!");
            //    //uiCanvas.SetActive(true);
            //    text_message = "While you are walking, click shift key to speed up for a short amount of time to dodge incoming bullets.";
            //    StartCoroutine(AnimateSpeech(text_message, "mutations"));
            //}
        }

        LevelEndTrigger levelEnd = other.gameObject.GetComponent<LevelEndTrigger>();

        if (levelEnd != null)
        {
            Vector3 playerPosition = this.transform.position;

            currScene = SceneManager.GetActiveScene().name;
            if ((foodCount < 5) && (currScene == "SampleScene"))
            {
                //FindObjectOfType<LevelEndTrigger>().OnLevelComplete("I need more food!");
                currDialog = "I NEED MORE FOOD!";
                FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog);
            }
            else
            {
                endDialogue = true;
            }


            // only if player has enough food
            if (endDialogue) {
                if (currScene == "SampleScene")
                {
                    currDialog = "ONTO THE CITY!";
                    FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog);
                    StartCoroutine(LevelChangeWait(1.5f));
                }
                else // if (currScene == "CITY")
                {
                    currDialog = "ONTO THE ROOFTOP!";
                    FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog);
                    StartCoroutine(LevelChangeWait(2.2f));
                }
                //FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog);
                //StartCoroutine(LevelChangeWait(1.5f));
            }
            
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
            //Vector3 bulletOffset = new Vector3(0, 0.2f, 0);     // Align bullets with gun
            //Vector3 weaponEndOffset = new Vector3(0, -0.3f, 0); // Align weapon end with gun

            if (guntransform != null)
            {
                GameObject gunObject = guntransform.gameObject;

                if (isFacingLeft)
                {
                    guntransformGun = guntransform.position; //+ bulletOffset; // Position the stream of bullets
                    //weaponend.localPosition += weaponEndOffset; // Position the weapon end
                    shootAnimator.SetTrigger("Shoot");
                    Instantiate(bulletPrefab, guntransformGun, Quaternion.identity);

                    CineMCamShake.Instance.ShakeCamera(3f, .3f);


                    //audioSource.PlayOneShot(shootSound);

                    // If did not switch gun direction, then reset weapon end for next time
                    //if (PrevShootLeft)
                    //{
                    //    weaponend.localPosition -= weaponEndOffset;
                    //    guntransformGun -= bulletOffset; //???oct 24 t e m p
                    //}

                    PrevShootLeft = true;
                    PrevShootRight = false;
                }
                else // Player is facing right
                {
                    // If switched gun direction, then reposition weapon end
                    if (PrevShootLeft)
                    {
                        //weaponend.localPosition -= weaponEndOffset;
                        shootAnimator.SetTrigger("Shoot");

                        PrevShootRight = true;
                        PrevShootLeft = false;

                        Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
                    }
                    else
                    {
                        shootAnimator.SetTrigger("Shoot");
                        Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
                    }
                    CineMCamShake.Instance.ShakeCamera(3f, .3f);
                }
            }
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
        audioSource.PlayOneShot(damageSound);

        //Flash when taking damage
        flashSpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.12f);
        flashSpriteRenderer.material = originalMaterial;
        flashroutine = null;
    }

    public IEnumerator WalkSounds()
    {
        while (true)
        {
            if (isMoving && walksoundtimer == 1)
            {
                //Play sound for walking
                walksoundnum = Random.Range(1, 5);
                if (walkstep == 1)
                {
                    if (walksoundnum == 1) audioSource.PlayOneShot(walkSound1);
                    else if (walksoundnum == 2) audioSource.PlayOneShot(walkSound3);
                    else if (walksoundnum == 3) audioSource.PlayOneShot(walkSound5);
                    else audioSource.PlayOneShot(walkSound7);
                    walkstep = 2;
                }
                else
                {
                    if (walksoundnum == 1) audioSource.PlayOneShot(walkSound2);
                    else if (walksoundnum == 2) audioSource.PlayOneShot(walkSound4);
                    else if (walksoundnum == 3) audioSource.PlayOneShot(walkSound6);
                    else audioSource.PlayOneShot(walkSound8);
                    walkstep = 1;
                }
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    public IEnumerator DashCooldown()
    {
        isDashing = false;
        StartCoroutine(SquishDown(0.02f));
        yield return new WaitForSeconds(dashCooldown);
        walksoundtimer = 1;
        canDash = true;
        audioSource.PlayOneShot(dashrecharged);
    }

    public IEnumerator SquishDown(float duration)
    {
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y * 0.75f, originalScale.z);
        float elapsedTime = 0;
        originalPosition = squishTransform.position; // Store the original position

        //Main Squish
        while (elapsedTime < duration)
        {
            float lerpFactor = elapsedTime / duration;
            Vector3 newScale = Vector3.Lerp(originalScale, targetScale, lerpFactor);

            // Adjust the y position to keep the bottom of the sprite in place
            //float heightDifference = newScale.y - targetScale.y;
            //Vector3 newPosition = squishTransform.position;
            //newPosition.y -= heightDifference / 2;
            //squishTransform.position = newPosition;

            squishTransform.localScale = newScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        squishTransform.localScale = targetScale;
        StartCoroutine(SquishUp(0.1f));
    }

    public IEnumerator SquishUp(float duration)
    {
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y * 0.75f, originalScale.z);
        float elapsedTime = 0;
        originalPosition = squishTransform.position; // Store the original position

        while (elapsedTime < duration)
        {
            float lerpFactor = elapsedTime / duration;
            Vector3 newScale = Vector3.Lerp(targetScale, originalScale, lerpFactor);
            
            // Adjust the y position to keep the bottom of the sprite in place
            //float heightDifference = originalHeight - newScale.y;
            //Vector3 newPosition = squishTransform.position;
            //newPosition.y += heightDifference / 2;
            //squishTransform.position = newPosition;
            
            squishTransform.localScale = newScale;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        squishTransform.localScale = originalScale;
        //squishTransform.position = originalPosition;
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

    //**
    //ordering layer 
    //void UpdateSortingOrder()
    //{
    //    // Access the player's SpriteRenderer
    //    SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();
    //    //int baseOrder = 1;

    //    if (playerRenderer != null)
    //    {
    //        // Invert the player's Y position to set sorting order (adjust multiplier if needed)
    //        playerRenderer.sortingLayerName = "Player";
    //        playerRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100) + 1;
    //    }
    //}



}