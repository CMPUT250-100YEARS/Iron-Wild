using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 18f;
    private float lifeTime = 1.5f;

    public float bulletDamage = 20f;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody2D bullet;

    private float rotate;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        bullet = GetComponent<Rigidbody2D>();

        mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePointer - transform.position;

        rotate = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; //update direction visuals
        transform.rotation = Quaternion.Euler(0, 0, rotate + 90);
        
        bullet.velocity = new Vector3(direction.x, direction.y).normalized * bulletSpeed;


        Destroy(gameObject, lifeTime);

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.one * Time.deltaTime;

    }



    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger!");
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        EnemyTurret enemyTurret = other.gameObject.GetComponent<EnemyTurret>();
        EnemyJuggernaut enemyJuggernaut = other.gameObject.GetComponent<EnemyJuggernaut>();
        MiniBoss miniBoss = other.gameObject.GetComponent<MiniBoss>();

        if (enemy != null)
        {
            enemy.takeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (enemyTurret != null)
        {
            enemyTurret.takeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (enemyJuggernaut != null)
        {
            enemyJuggernaut.takeDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (miniBoss != null)
        {
            miniBoss.takeDamage(bulletDamage);
            Destroy(gameObject);
        }

    }
}
