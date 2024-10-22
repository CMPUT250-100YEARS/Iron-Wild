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


    public GameObject bulletPrefab;
    private float fireRate = 0.2f;
    public float cooldown;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody2D rb;
    public LayerMask SolidObjectsLayer; //the foreground

    public float foodCount = 0;
    public string currScene; //???oct20
    public string currDialog; //???oct20
    public Vector3 startPosition;  //???

    private bool hasAbility_Dash;
    private float dashSpeed;

    //public float dashSpeed = 20f; // Speed of the dash
    private float dashDuration = 0.1f; // Duration of the dash
    private bool isDashing = false;
    private float dashTime;

    private Vector2 dashDirection;

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


    void Start()
    {
        AnimationSetup();
        _priorPosition = transform.position;  // ADDED
        audioSource = gameObject.GetComponent<AudioSource>();

        aimTransform = transform.Find("Aim");
        Transform weaponend = aimTransform.Find("weaponend");
        shootAnimator = weaponend.GetComponent<Animator>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        hasAbility_Dash = false;
        dashSpeed = Speed * 4;

        foodCount = 0;

        //PlayerPrefs.SetInt("numHearts", 2); // set num hearts initially
        //PlayerPrefs.Save();


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


        //int maxLives = 6;  //???oct20 BAD RESETS TO 6 HEARTS GOING ONTO CITY SCENE
        //PlayerPrefs.SetInt("numHearts", maxLives); //???oct20
        //PlayerPrefs.Save(); //???oct20

        Heart heartScript = FindObjectOfType<Heart>();
        heartScript.InitializeHearts();
        FindObjectOfType<Heart>().UpdateHearts();
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
        if (angle < 0 ) { angle += 360f; }

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
                    if (!IsCollidingWith(targetPos))
                    {

                        if (isDashing)
                        {
                            //Debug.Log("isDashing" + isDashing);
                            //Debug.Log("HEREEEEE!!! speed increased to True");
                            transform.position += inputDirection.normalized * Time.deltaTime * dashSpeed;
                        }
                        else { transform.position += inputDirection.normalized * Time.deltaTime * Speed; }
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
            if (cooldown >= 0.0f){
                cooldown -= Time.deltaTime;
            }
        }


        // Temporary, to test Heart system
        //if (Input.GetKeyDown(KeyCode.H)) // For testing, press H to take damage ???
        //{                                           
        //    FindObjectOfType<Heart>().TakeDamage();
        //}

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

    public IEnumerator LevelChangeWait(float time)
    {
        yield return new WaitForSeconds(time);


        currScene = SceneManager.GetActiveScene().name; //???oct20
        //Debug.Log("LevelChangeWait curr scene " + currScene);  //???oct20
        if (currScene == "SampleScene")  //???oct20
        {
            Debug.Log("LevelChangeWait Load CITY scene");  //???oct20
            SceneManager.LoadScene("CITY");  //???oct20
        }
        else // if (currScene == "CITY") //???oct20
        {
            Debug.Log("LevelChangeWait Load RoofTop scene" );  //???oct20
            SceneManager.LoadScene("RoofTop");  //???oct20
        }


        //SceneManager.LoadScene("RoofTop"); //???oct20

        //Time.timeScale = 1f;
        endDialogue = false;
    }

    void OnTriggerEnter2D(Collider2D other){
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
            //pickup.Reset();
        }



        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy!=null){

            //if (playerDead != null)
            //{
            //    playerDead.Play();
            //}
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            GameOverManager gameOver = FindObjectOfType<GameOverManager>();
            FindObjectOfType<Heart>().TakeDamage();
            //gameOver.PlayerLost();
            //gameOver.PlayerLost("SampleScene");

            //WaterManager water = FindObjectOfType<WaterManager>();
            //water.SetWater();
        }



        PuddleScript puddle = other.gameObject.GetComponent<PuddleScript>();
        if (puddle!= null){
            
            Debug.Log("found puddle!");
            FindObjectOfType<WaterManager>().IncreaseWater(20f); //???
            Destroy(puddle.gameObject);

            if ((waterSound != null) && (audioSource != null))
            {
                audioSource.PlayOneShot(waterSound);
            }
        }


        FoodObject food = other.gameObject.GetComponent<FoodObject>();
        //FoodImage foodImage = other.gameObject.GetComponent<FoodImage>();

        if (food != null)
        {
            foodCount++;
            FindObjectOfType<FoodImage>().FoundFoods();
            Destroy(food.gameObject);

        }

        LevelEndTrigger levelEnd = other.gameObject.GetComponent<LevelEndTrigger>();
        //FoodImage foodImage = other.gameObject.GetComponent<OnTriggerEnter>();

        if (levelEnd != null)
        {
            //FindObjectOfType<LevelEndTrigger>().ShowSpeechBubble();
            Vector3 playerPosition = this.transform.position; //???

            //if (foodCount < 2)
            //{
            //    FindObjectOfType<LevelEndTrigger>().OnLevelComplete("I need more food!");
            //} else
            //{
                endDialogue = true;



            currScene = SceneManager.GetActiveScene().name; //???oct20
                                                            //Debug.Log("LevelChangeWait curr scene " + currScene);  //???oct20
            if (currScene == "SampleScene")  //???oct20
            {
                //Debug.Log("currDialog CITY ");  //???oct20
                currDialog = "ONTO THE CITY";  //???oct20
            }
            else // if (currScene == "CITY") //???oct20
            {
                //Debug.Log("currDialog ROOFTOP");  //???oct20
                currDialog = "ONTO THE ROOFTOP";  //???oct20
            }
            FindObjectOfType<LevelEndTrigger>().OnLevelComplete(currDialog); //???oct20

            //FindObjectOfType<LevelEndTrigger>().OnLevelComplete("Onto the rooftop!"); //???oct20



            Debug.Log("#1hearts" + PlayerPrefs.GetInt("numHearts"));
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
            if (guntransform != null)
            {
                shootAnimator.SetTrigger("Shoot");
                GameObject gunObject = guntransform.gameObject;
                Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
                CineMCamShake.Instance.ShakeCamera(5f, .1f);
                audioSource.PlayOneShot(shootSound);
            }
            //GameObject.Instantiate(bulletPrefab, guntransform.position, guntransform.rotation, gunObject.transform);
        }
    }

    public float getSpeed()
    {
        return Speed;
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
