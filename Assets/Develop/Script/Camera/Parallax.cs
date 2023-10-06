using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Cinemachine;

public class Parallax : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float startX, startY;

    [SerializeField]
    private GameObject cam;
    
    [Space (10f)]
    [Header("스크롤 속도")]
    [SerializeField, Tooltip("이미지가 수평으로 움직이는 속도를 조절합니다.")]
    private float horizontalSpeed;
    [SerializeField, Tooltip("이미지가 수직으로 움직이는 속도를 조절합니다.")]
    private float vericalSpeed;
    
    [Space (10f)]
    [SerializeField]
    private bool isVerticalChange;
    [SerializeField]
    private float previousCamY;
    
    [SerializeField]
    private float nextCamY;

    private float xDist;
    private float yDist;

    private float camDeflection = 0f;

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
