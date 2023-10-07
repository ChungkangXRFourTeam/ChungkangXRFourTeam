using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Tooltip("오브젝트의 수평 이동 속도를 조절합니다.")] private float _horizontalFactor;
    [SerializeField, Tooltip("오브젝트의 수직 이동 속도를 조절합니다.")] private float _verticalFactor;

    public void MoveX(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * _horizontalFactor;

        transform.localPosition = newPos;
    }
    
    public void MoveY(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= delta * _verticalFactor;

        transform.localPosition = newPos;
    }

}