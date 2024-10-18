using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraF : MonoBehaviour
{

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f; //how long it takes the camera to reach target
    private Vector3 velocity = Vector3.zero; //velocity of camera
   
    public Transform target;
    public Transform topRightBoundary;
    public Transform bottomLeftBoundary;

    private float minX, maxX, minY, maxY;

    //zoom ***********n
    public float zoomOutSize = 10f;
    public float normalSize = 5f;
    public float zoomSpeed = 1f;

    private bool shouldZoomOut = false;
    
    
    //*****************u

    // Start is called before the first frame update
    void Start()
    {
        
        //boundary from blank GameObject
        minX = bottomLeftBoundary.position.x; //(-0.7,-0.8)
        maxX = topRightBoundary.position.x; //(28.9,24.5)
        minY = bottomLeftBoundary.position.y;
        maxY = topRightBoundary.position.y;

        Camera.main.orthographicSize = normalSize;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;

            //camera boundary
            //float halfHeight = Camera.main.orthographicSize;
            //float halfWidth = halfHeight * Camera.main.aspect;

            //position of camera should be between max and min
            //targetPosition.x = Mathf.Clamp(targetPosition.x, minX + halfWidth, maxX - halfWidth);
            //targetPosition.y = Mathf.Clamp(targetPosition.y, minY + halfHeight, maxY - halfHeight);
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX , maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY , maxY );

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        }
        else
        {
            Debug.LogWarning("Target is not assigned in the Inspector!");
        }

        //********n
        if (shouldZoomOut)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomOutSize, Time.deltaTime * zoomSpeed);
        }
        else
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, normalSize, Time.deltaTime * zoomSpeed);
        }
        //**************u
    }
    //***********n
    public void TriggerZoomOut(bool zoomOut)
    {
        shouldZoomOut = zoomOut;
    }
    //****************u
}
