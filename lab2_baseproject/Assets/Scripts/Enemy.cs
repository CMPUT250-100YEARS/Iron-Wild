using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AnimatedEntity
{

    private float RangeX = 4, RangeY = 4;
    private float EnemyHealth = 100.0f;

    private Transform detectionZone;
    private Transform target;
    private float followSpeed = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        
        detectionZone = transform.Find("DetectionZone");
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        if (target == null)
        {
            //pass
        }
        else if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize(); // Normalize the direction to get a unit vector

            // Move the enemy towards the player
            transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        }
        //transform.position += new Vector3(Random.Range(-1 * RangeX, RangeX), Random.Range(-1 * RangeY, RangeY)) * Time.deltaTime;
    }

    public void takeDamage(float damage)
    {
        EnemyHealth -= damage;
        Debug.Log("Enemy Health is now " + EnemyHealth);

        if (EnemyHealth <= 0) { Destroy(gameObject); }
    }

    public void OnDetectionTriggerEnter(Collider2D other)
    {
        Debug.Log("Now I follow!");
        target = other.transform;
        
    }

    public void OnDetectionTriggerExit(Collider2D other)
    {
        target = null;
        Debug.Log("Now I stop!");
    }

}
