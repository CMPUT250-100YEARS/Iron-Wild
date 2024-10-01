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




    void Start()
    {
        AnimationSetup();
        audioSource = gameObject.GetComponent<AudioSource>();
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
        if (Input.GetKey(KeyCode.Space) && cooldown <= 0.0f)
        {
            cooldown = fireRate;
            GameObject.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            cooldown -= Time.deltaTime;
        }

    }

    void OnTriggerEnter(Collider other){
        Debug.Log("Atleast it works!");

        Pickup pickup = other.gameObject.GetComponent<Pickup>();

        audioSource.Play();
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
            Debug.Log("Found Puddle!");

            FindObjectOfType<WaterManager>().IncreaseWater(5f);

        }
    }
    
}
