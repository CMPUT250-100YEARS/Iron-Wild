using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : AnimatedEntity
{

    //private float RangeX = 4, RangeY = 4;
    public float EnemyHealth;
    public AudioSource audioSource;

    private Transform detectionZone;
    private Transform target;
    private float followSpeed = 0f;
    public float avoidanceRadius = 1.5f; // radius to avoid other enemy

    public GameObject bulletEnemyPrefab;
    public Transform enemyBulletPos;

    public LayerMask SolidObjectsLayer;
    public LayerMask EnemyLayer;

    //Sound Effects
    public AudioClip clunk1;
    public AudioClip clunk2;
    public AudioClip clunk3;
    public AudioClip deadblast;
    public AudioClip shoot1;
    public AudioClip shoot2;
    public AudioClip alert;

    //Sprite list based on direction
    public List<Sprite> BackSpriteList;
    public List<Sprite> RightSpriteList;
    public List<Sprite> LeftSpriteList;
    public List<Sprite> FrontSpriteList;
    public List<Sprite> IdleSpriteList;

    public Material flashMaterial; //Colour for the enemy to flash when taking damage
    private Material originalMaterial;
    private SpriteRenderer flashSpriteRenderer;
    private Coroutine flashroutine;


    public List<Sprite> InterruptedCycle;
    private List<Sprite> currentSpriteCycle;
    private bool isMoving = false;
    private bool alive = true;

    private int range;
    private Vector3 direction; // Store the current direction
    private float updateInterval = 1.0f; // Shoot every interval
    private bool seesplayer = false;


    // Start is called before the first frame update
    void Start()
    {
        detectionZone = transform.Find("DetectionZone");
        AnimationSetup();
        target = null;
        range = 4;
        StartCoroutine(UpdateDirection()); // Start the direction update coroutine

        //Set up the sprite renderer for flash effects
        flashSpriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = flashSpriteRenderer.material;
    }

    //Finds the distance between the player and the enemy
    int getDistance()
    {
        float dist = Vector3.Distance(target.position, transform.position);
        //Returns 1 if the enemy is in close range, 2 if in medium range, 3 if in long range, 4 if outside of range.
        if (dist < 3.2f) return 1;
        else if (dist < 7.5f) return 2;
        else if (dist < 16f) return 3;
        else return 4;
    }

    // Coroutine to update direction every interval
    private IEnumerator UpdateDirection()
    {
        while (true)
        {
            if (target != null)
            {
                Vector3 new_direction = target.position - transform.position;

                range = getDistance();
                if (range <= 3 && seesplayer == false)
                {
                    audioSource.PlayOneShot(alert);
                    yield return new WaitForSeconds(0.5f);
                    seesplayer = true;
                }

                new_direction.Normalize(); // Normalize the direction to get a unit vector
                direction = new_direction;



                //Fire a bullet at each step

                if (range <= 3 && seesplayer == true)
                {
                    int random_sound = Random.Range(1, 3);
                    if (random_sound == 1) audioSource.PlayOneShot(shoot2);
                    else audioSource.PlayOneShot(shoot1);

                    Instantiate(bulletEnemyPrefab, enemyBulletPos.position, Quaternion.identity);
                }
                else seesplayer = false;
            }

            yield return new WaitForSeconds(updateInterval); // Wait for about 1 interval before updating again
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (alive == true)
        {
            AnimationUpdate();
            if (target == null || !seesplayer)
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
        EnemyFlash();

        if (EnemyHealth <= 0)
        {
            alive = false;
            StartCoroutine(Death());
        }

        else
        {
            int random_sound = Random.Range(1, 4);
            if (random_sound == 1) audioSource.PlayOneShot(clunk3);
            else if (random_sound == 2) audioSource.PlayOneShot(clunk2);
            else audioSource.PlayOneShot(clunk1);
        }
    }

    private IEnumerator Death()
    {
        audioSource.PlayOneShot(deadblast);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
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


    public void EnemyFlash()
    {
        //Call the flash, if it is not currently playing
        if (flashroutine != null)
        {
            StopCoroutine(flashroutine);
        }

        flashroutine = StartCoroutine(hitFlash());
    }

    public IEnumerator hitFlash()
    {
        //Flash when taking damage
        flashSpriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.12f);
        flashSpriteRenderer.material = originalMaterial;
        flashroutine = null;
    }

}