using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{   
    public float RangeX = 5, RangeY = 4;

    public void Reset(){
        transform.position = new Vector3(Random.Range(-1*RangeX, RangeX), Random.Range(-1*RangeY, RangeY));
    }
}
