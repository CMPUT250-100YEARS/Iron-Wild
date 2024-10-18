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

    public LayerMask SolidObjectsLayer;

    //Sprite list based on direction
    public List<Sprite> BackSpriteList;
    public List<Sprite> RightSpriteList;
    public List<Sprite> LeftSpriteList;
    public List<Sprite> FrontSpritList;

    
    public List<Sprite> InterruptedCycle;
    private List<Sprite> currentSpriteCycle;
    bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        
        detectionZone = transform.Find("DetectionZone");
        AnimationSetup();
        target = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        if (target == null)
        {
            isMoving = false;
            //pass
        }
        else if (target != null)
        {
            isMoving = true;
            Vector3 direction = target.position - transform.position;
            direction.Normalize(); // Normalize the direction to get a unit vector

            //angle of enemy compared to player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle < 0) { angle += 360f; }

            //Determine SpriteList based on angle
            if (angle > 45f && angle <= 135f)
            {
                currentSpriteCycle = BackSpriteList; ;
            }

            else if (angle > 135f && angle <= 225f)
            {
                currentSpriteCycle = RightSpriteList;
            }

            else if (angle > 225f && angle <= 315f)
            {
                currentSpriteCycle = FrontSpritList;
            }

            else if (angle > 315f || angle <= 45f)
            {
                currentSpriteCycle = LeftSpriteList;
            }




            // Move the enemy towards the player
            Vector3 targetPos = transform.position + direction * followSpeed * Time.deltaTime;

            //transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
            if (!IsCollidingWith(targetPos))
            {
                isMoving = true;
                transform.position = targetPos;
                
            }
            else
            {
                isMoving = false;
            }

            SetMovementDirection(isMoving);
            SetCurrentAnimationCycle(currentSpriteCycle);
        }
        //transform.position += new Vector3(Random.Range(-1 * RangeX, RangeX), Random.Range(-1 * RangeY, RangeY)) * Time.deltaTime;
    }

    bool IsCollidingWith(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.1f, SolidObjectsLayer) != null)
        {
            return true; //player colliding with an object in foreground
        }
        return false; //no collision - player can move
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
