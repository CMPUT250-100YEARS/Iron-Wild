using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : AnimatedEntity
{
    
    public float Speed = 5;
    private AudioSource audioSource;

    public List<Sprite> InterruptedCycle;

    void Start()
    {
        AnimationSetup();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        //Movement controls
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            transform.position+= Vector3.up*Time.deltaTime*Speed;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            transform.position+= Vector3.left*Time.deltaTime*Speed;
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            transform.position+= Vector3.down*Time.deltaTime*Speed;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            transform.position+= Vector3.right*Time.deltaTime*Speed;
        }

    }

    void OnTriggerEnter(Collider other){
        Pickup pickup = other.gameObject.GetComponent<Pickup>();

        audioSource.Play();
        if(pickup!=null){

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
    }

}
