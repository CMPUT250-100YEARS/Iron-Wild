using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 10f;
    private float lifeTime = 3f;

    private Camera mainCamera;
    private Vector3 mousePointer;

    private Rigidbody bullet;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        bullet = GetComponent<Rigidbody>();

        mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePointer - transform.position;

        
        bullet.velocity = new Vector3(direction.x, direction.y).normalized * bulletSpeed;
        Debug.Log(bullet.velocity);


        Destroy(gameObject, lifeTime);

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.one * Time.deltaTime;

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger!");
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Destroy();
            Destroy(gameObject);
        }

    }
}
