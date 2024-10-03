using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public static float foodNum = 1;
    Vector3 newPos = new Vector3(-1, -4, 0);
    Vector3 newPos2 = new Vector3(1, -4, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveFood()
    {
        if (transform.position != newPos)
        {
            if (foodNum == 1)
            {
                transform.position = newPos;
                foodNum++;
            }
            else if (foodNum == 2)
            {
                transform.position = newPos2;
                foodNum++;
            }
        }
    }
}
