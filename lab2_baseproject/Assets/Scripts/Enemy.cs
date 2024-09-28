using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : AnimatedEntity
{

    private float RangeX = 4, RangeY = 4;


    // Start is called before the first frame update
    void Start()
    {
        AnimationSetup();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
        transform.position += new Vector3(Random.Range(-1 * RangeX, RangeX), Random.Range(-1 * RangeY, RangeY)) * Time.deltaTime;
    }
}
