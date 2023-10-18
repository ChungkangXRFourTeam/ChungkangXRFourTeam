using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerType
{
    Background,
    Middleground,
    Foreground
}

[AddComponentMenu("패럴랙스 스크롤/패럴랙스 레이어 스크립트")]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Tooltip("오브젝트의 타입을 정합니다.")]
    private LayerType _layerType;
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