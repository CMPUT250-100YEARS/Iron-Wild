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


    




    void Start()
    {
        AnimationSetup();
        audioSource = gameObject.GetComponent<AudioSource>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        //Movement controls |start with player not moving
        bool isMoving = false; 
        bool isMovingRight = false; 

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            transform.position+= Vector3.up*Time.deltaTime*Speed;
            isMoving = false;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            transform.position+= Vector3.left*Time.deltaTime*Speed;
            isMoving = true; //checking
            isMovingRight = false; //moving Left
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            transform.position+= Vector3.down*Time.deltaTime*Speed;
            isMoving = false;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            transform.position+= Vector3.right*Time.deltaTime*Speed;
            isMoving = true;
            isMovingRight = true; //moving right
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

            FindObjectOfType<WaterManager>().IncreaseWater(5f);
        }


        Food food = other.gameObject.GetComponent<Food>();

        if (food != null)
        {
            food.MoveFood();
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
