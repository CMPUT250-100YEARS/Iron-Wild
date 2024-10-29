using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BulletEnemy : MonoBehaviour
{
    public float enemyBulletSpeed = 10f;
    private float lifeTime = 1.5f;

    private Transform target;
    private Rigidbody2D bullet;

    void Start()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        bullet = GetComponent<Rigidbody2D>();

        bullet.velocity = new Vector3(direction.x, direction.y).normalized * enemyBulletSpeed;

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.one * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            //LoseLife();
            Destroy(gameObject);
        }
    }
}
