﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 18f;
    private float lifeTime = 0.62f;

    public float bulletDamage = 20f;

    //private Camera mainCamera;
    //private Vector3 mousePointer;

    private Rigidbody2D bullet;

    private float rotate;

    private GameObject player;
    private Transform aim;
    private Transform gunOrWeaponend;


    // Start is called before the first frame update
    void Start()
    {
        //Version 1
        //mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // Version 2
        player = GameObject.Find("Player");
        aim = player.transform.Find("Aim");
        gunOrWeaponend = aim.Find("Gun/weaponend");

        // Version 3
        //player = GameObject.Find("Player");
        //aim = player.transform.Find("Aim");



        bullet = GetComponent<Rigidbody2D>();


        //V1
        // mousePointer = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 direction = mousePointer - transform.position;

        // v2
        Vector3 direction = gunOrWeaponend.position - aim.position;

        // v1 and v2
        rotate = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; //update direction visuals
        transform.rotation = Quaternion.Euler(0, 0, rotate + 90);

        //transform.rotation =  Quaternion.Euler(0, 0, aim.rotation.z+ 90);
        
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
        Debug.Log("Bullet collider entered with: " + other.gameObject.name);

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

        if (other.gameObject.name == "SolidObjects")
        {
            // TODO: add animation on wall hit and add different sound maybe?
            Debug.Log("Enterring this loop!");
            Destroy(gameObject);
        }

    }
}
