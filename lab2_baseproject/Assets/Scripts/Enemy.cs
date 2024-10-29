using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AnimatedEntity
{

    //private float RangeX = 4, RangeY = 4;
    private float EnemyHealth = 100.0f;

    private Transform detectionZone;
    private Transform target;
    private float followSpeed = 3.5f;
    public float avoidanceRadius = 1.5f; // radius to avoid other enemy

    public GameObject bulletEnemyPrefab;

    public LayerMask SolidObjectsLayer;
    public LayerMask EnemyLayer;

    //Sprite list based on direction
    public List<Sprite> BackSpriteList;
    public List<Sprite> RightSpriteList;
    public List<Sprite> LeftSpriteList;
    public List<Sprite> FrontSpriteList;
    public List<Sprite> IdleSpriteList;


    public List<Sprite> InterruptedCycle;
    private List<Sprite> currentSpriteCycle;
    bool isMoving = false;

    private Vector3 direction; // Store the current direction
    private float updateInterval = 0.8f; // Update direction every second


    // Start is called before the first frame update
    void Start()
    {
        detectionZone = transform.Find("DetectionZone");
        AnimationSetup();
        target = null;
        StartCoroutine(UpdateDirection()); // Start the direction update coroutine
    }

    //Finds the distance between the player and the enemy
    int getDistance()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        //Returns 1 if the enemy is in close range, 2 if in medium range, 3 if in long range.
        if (dist < 3.2f) return 1;
        else if (dist < 6f) return 2;
        else return 3;
    }

    // Coroutine to update direction every interval
    private IEnumerator UpdateDirection()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                if (target != null)
                {
                    Vector3 new_direction = target.position - transform.position;

                    int range = getDistance();
                    if (range == 1)
                    {
                        new_direction = -new_direction; // Enemy runs from player
                    }
                    else if (range == 2)
                    {
                        int randomValue = Random.Range(1, 7);
                        if (randomValue == 1) new_direction = -new_direction;
                        else if (randomValue <= 3) new_direction = new Vector3(-new_direction.y, new_direction.x, 0);
                        else if (randomValue <= 5) new_direction = new Vector3(new_direction.y, -new_direction.x, 0);
                    }
                    // Long range or random value of 6: no further updates needed

                    new_direction.Normalize(); // Normalize the direction to get a unit vector
                    direction = new_direction;
                }

                yield return new WaitForSeconds(Random.Range(updateInterval*0.5f, updateInterval*1.3f)); // Wait for about 1 interval before updating again
            }

            //Every two intervals, fire a bullet
            //Instantiate(bulletEnemyPrefab);
        }
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
                currentSpriteCycle = FrontSpriteList;
            }

            else if (angle > 315f || angle <= 45f)
            {
                currentSpriteCycle = LeftSpriteList;
            }




            // Move the enemy towards the player
            Vector3 targetPos = transform.position + direction * followSpeed * Time.deltaTime;

            //transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

            //transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
            //AvoidOtherEnemies(ref targetPos);

            if (!IsCollidingWith(targetPos))
            {
                isMoving = true;
                transform.position = targetPos;

            }
            else
            {
                //isMoving = false;
                //currentSpriteCycle = IdleSpriteList;
                direction = -direction;
            }

            SetMovementDirection(isMoving);
            SetCurrentAnimationCycle(currentSpriteCycle);
        }
        //transform.position += new Vector3(Random.Range(-1 * RangeX, RangeX), Random.Range(-1 * RangeY, RangeY)) * Time.deltaTime;
    }

    //void AvoidOtherEnemies(ref Vector3 targetPos)
    //{
    //    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius, EnemyLayer);
    //    foreach(Collider2D collider in colliders)
    //    { 
    //        if (collider.gameObject != gameObject)
    //        {
    //            Vector3 awayDirection = (transform.position - collider.transform.position).normalized;
    //            float distance = Vector3.Distance(transform.position, collider.transform.position);
    //            targetPos += awayDirection * (avoidanceRadius - distance) * 1f;
    //        }
    //    }    
    //}

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