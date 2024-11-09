using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BulletEnemy : MonoBehaviour
{
    public float enemyBulletSpeed;
    private float lifeTime = 1f;

    private GameObject player;

    private Rigidbody2D bullet;

    private float rotate;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bullet = GetComponent<Rigidbody2D>();

        Vector3 direction = (player.transform.position - transform.position).normalized;

        rotate = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; //update direction visuals
        transform.rotation = Quaternion.Euler(0, 0, rotate + 90);

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

        else if (other.gameObject.name == "SolidObjects")
        {
            // TODO: add animation and sound effects maybe;
            Destroy(gameObject);
        }
    }
}
