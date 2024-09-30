using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 10f;
    private float lifeTime = 3f;

    private Enemy enemy;
    private Rigidbody bullet;

    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * bulletSpeed;

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
