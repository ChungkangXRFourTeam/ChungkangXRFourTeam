using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("패럴랙스 스크롤/패럴랙스 카메라")]
public class ParallaxCamera : MonoBehaviour 
{
    public delegate void ParallaxCameraDelegate(float deltaMovementX, float deltaMovementY);
    public ParallaxCameraDelegate onCameraTranslate;

    private float oldPositionX;
    private float oldPositionY;
    void Start()
    {
        oldPositionX = transform.position.x;
        oldPositionY = transform.position.y;
    }


    private void LateUpdate()
    {
        if (transform.position.x != oldPositionX && transform.position.y != oldPositionY)
        {
            if (onCameraTranslate != null)
            {
                float deltaX = oldPositionX - transform.position.x;
                float deltaY = oldPositionY - transform.position.y;
                onCameraTranslate(deltaX,deltaY);
            }

            oldPositionX = transform.position.x;
            oldPositionY = transform.position.y;
        }
    }
}
