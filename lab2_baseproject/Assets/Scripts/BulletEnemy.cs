using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BulletEnemy : MonoBehaviour
{
    public float enemyBulletSpeed;
    private float lifeTime = 1.5f;

    private GameObject player;

    private Rigidbody2D bullet;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bullet = GetComponent<Rigidbody2D>();

        Vector3 direction = (player.transform.position - transform.position).normalized;
        bullet.velocity = new Vector3(direction.x, direction.y).normalized * enemyBulletSpeed;

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.one * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Trigger!");
            
            FindObjectOfType<Player>().PlayerFlash();
            GameOverManager gameOver = FindObjectOfType<GameOverManager>();
            FindObjectOfType<Heart>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
