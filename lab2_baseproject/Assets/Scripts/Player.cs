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
    


    void Start()
    {
        AnimationSetup();
        audioSource = gameObject.GetComponent<AudioSource>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();

        //Movement controls |start with player not moving
        bool isMoving = false; 
        bool isMovingRight = false;
        Vector2 inputDirection = Vector2.zero;

        //check input WASD and store direction
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){

            //transform.position+= Vector3.up*Time.deltaTime*Speed;
            inputDirection = Vector2.up;
            isMoving = true;
        }

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            //transform.position+= Vector3.left*Time.deltaTime*Speed;
            inputDirection = Vector2.left;
            isMoving = true; //checking
            isMovingRight = false; //moving Left
        }

        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            //transform.position+= Vector3.down*Time.deltaTime*Speed;
            inputDirection = Vector2.down;
            isMoving = true;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            //transform.position+= Vector3.right*Time.deltaTime*Speed;
            inputDirection = Vector2.right;
            isMoving = true;
            isMovingRight = true; //moving right
        }

        //If isMoving ==true, check for collision(foreground) then move
        if (isMoving)
        {
            Vector3 targerPos = rb.position + inputDirection * Time.deltaTime * Speed; //inputDirection Vector2.up/down/...
            if (!IsCollidingWith(targerPos))
            {
                rb.MovePosition(rb.position + inputDirection * Time.deltaTime * Speed);
            }
        }

        // call SetMovementDirection
        SetMovementDirection(isMovingRight, isMoving);

        
        
        
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
            Interrupt(InterruptedCycle);
            if (audioSource != null)
            {
                audioSource.Play();
            }
            pickup.Reset();
        }



        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if(enemy!=null){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }



        PuddleScript puddle = other.gameObject.GetComponent<PuddleScript>();
        if (puddle!= null){
            Debug.Log("found puddle!");
            FindObjectOfType<WaterManager>().IncreaseWater(5f);
        }


        FoodObject food = other.gameObject.GetComponent<FoodObject>();
        //FoodImage foodImage = other.gameObject.GetComponent<FoodImage>();

        if (food != null)
        {
            FindObjectOfType<FoodImage>().FoundFoods();
            Destroy(food.gameObject);

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
            }
            //GameObject.Instantiate(bulletPrefab, guntransform.position, guntransform.rotation, gunObject.transform);
        }
    }
}
