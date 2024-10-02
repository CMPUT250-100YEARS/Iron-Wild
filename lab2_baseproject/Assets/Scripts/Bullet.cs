using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 20f;
    private float lifeTime = 1.5f;

    public float bulletDamage = 33.4f;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody2D bullet;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        bullet = GetComponent<Rigidbody2D>();

        mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePointer - transform.position;

        
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

        if (enemy != null)
        {
            enemy.takeDamage(bulletDamage);
            Destroy(gameObject);
        }

    }
}
