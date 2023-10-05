using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    // Start is called before the first frame update

    public float startX, startY;

    public GameObject cam;

    public float horizontalSpeed;
    public float vericalSpeed;

    public bool isVerticalChange;

    public float previousCamY;
    public float nextCamY;

    float xDist;
    float yDist;

    float camDeflection = 0f;

    private void Awake()
    {
        startX = transform.position.x;
        startY = transform.position.y;
        cam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Start()
    {
        nextCamY = cam.transform.position.y;
        yDist = startY;
    }

    private void FixedUpdate()
    {
        previousCamY = nextCamY;
        nextCamY = cam.transform.position.y;
        camDeflection = nextCamY - previousCamY;

        xDist = (cam.transform.position.x * horizontalSpeed);
        yDist += camDeflection * vericalSpeed;
    }


    private void LateUpdate()
    {
        if(isVerticalChange)
            transform.position = new Vector3(startX + xDist, yDist, transform.position.z);
        else
            transform.position = new Vector3(startX + xDist, startY, transform.position.z);
    }

}
