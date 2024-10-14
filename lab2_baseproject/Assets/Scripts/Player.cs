using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : AnimatedEntity
{
    
    public float Speed = 5;
    private AudioSource audioSource;

    public List<Sprite> InterruptedCycle;

    public GameObject bulletPrefab;
    public float fireRate = 0.5f;
    public float cooldown;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody2D rb;
    public LayerMask SolidObjectsLayer; //the foreground

    public float foodCount = 0;
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

    private List<Sprite> currentSpriteCycle;



    void Start()
    {
        AnimationSetup();
        audioSource = gameObject.GetComponent<AudioSource>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        hasAbility_Dash = false;
        dashSpeed = Speed * 4;

        foodCount = 0; //???
        //startPosition = transform.position; // Store the starting position //???
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

        //Determine SpriteList based on angle
        if (angle > 45f && angle <= 135f)
        {
            currentSpriteCycle = BackSpriteList;
        }

        else if (angle > 135f && angle <= 225f)
        {
            currentSpriteCycle = LeftSpriteList;
        }

        else if (angle > 225f && angle <= 315f)
        {
            currentSpriteCycle = FrontSpritList;
        }

        else if (angle > 315f || angle <= 45f)
        {
            currentSpriteCycle = RightSpriteList;
        }

        //check input WASD and store direction
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {

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

        // call SetMovementDirection
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
        if (Input.GetKeyDown(KeyCode.H)) // For testing, press H to take damage ???
        {                                           
            FindObjectOfType<Heart>().TakeDamage();
        }

        //// Temporary, to test Food system
        //if (Input.GetKeyDown(KeyCode.F)) // For testing, press F to take damage ???
        //{
        //    FindObjectOfType<FoodImage>().FoundFoods();
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

    bool IsCollidingWith(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, SolidObjectsLayer) != null)
        {
            return true; //player colliding with an object in foreground
        }
        return false; //no collision - player can move
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
            gameOver.PlayerLost();
            //WaterManager water = FindObjectOfType<WaterManager>();
            //water.SetWater();
        }



        PuddleScript puddle = other.gameObject.GetComponent<PuddleScript>();
        if (puddle!= null){
            
            Debug.Log("found puddle!");
            FindObjectOfType<WaterManager>().IncreaseWater(5f);
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

            if (foodCount < 2)
            {
                FindObjectOfType<LevelEndTrigger>().OnLevelComplete("I need more food!");
            } else
            {
                FindObjectOfType<LevelEndTrigger>().OnLevelComplete("Onto the next level!");
            }
            //FindObjectOfType<LevelEndTrigger>().ShowSpeechBubble(foodCount);

        }

    }

    void Shoot()
    {
        
        
        Transform aimtransform = transform.Find("Aim");
        if (aimtransform != null)
        {
            GameObject aimObject = aimtransform.gameObject;

            mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mousePointer - transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            aimtransform.rotation = Quaternion.Euler(0, 0, rotZ);

            Transform guntransform = aimtransform.Find("Gun");
            if (guntransform != null)
            {
                GameObject gunObject = guntransform.gameObject;
                Debug.Log("second if passed!");

                // TODO: Change the  bullet spawned direction gun to aim
                Instantiate(bulletPrefab, guntransform.position, Quaternion.identity);
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
        // Reset player state
        foodCount = 0;
        //startPosition = transform.position;
    }


}
