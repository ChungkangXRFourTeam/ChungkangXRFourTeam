using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum LayerType
{
    Background,
    Middleground,
    Foreground
}

[AddComponentMenu("패럴랙스 스크롤/패럴랙스 레이어")]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField, Tooltip("오브젝트의 타입을 정합니다.")]
    private LayerType _layerType;
    [SerializeField, Tooltip("오브젝트의 수평 이동 속도를 조절합니다.")] private float _horizontalFactor;
    [SerializeField, Tooltip("오브젝트의 수직 이동 속도를 조절합니다.")] private float _verticalFactor;
    private SpriteRenderer _renderer;
    private Rigidbody2D rigid;
    
    [SerializeField]
    private Transform _leftPoint;
    [SerializeField]
    private Transform _rightPoint;
    [SerializeField]
    private Transform _upPoint;
    [SerializeField]
    private Transform _downPoint;
    

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        if (_layerType == LayerType.Background)
        {
            float posX = transform.position.x;
            float posY = transform.position.y;
            _leftPoint.position = new Vector2((posX - (_renderer.size.x / 2) * transform.localScale.x),posY);
            _rightPoint.position = new Vector2((posX + (_renderer.size.x / 2) * transform.localScale.x),posY);
            _upPoint.position = new Vector2(posX,posY+ (_renderer.size.y / 2) * transform.localScale.y);
            _downPoint.position = new Vector2(posX,posY - (_renderer.size.y / 2)*transform.localScale.y);
        }
    }

    void SetLayerSize()
    {
        switch (_layerType)
        {
            case LayerType.Background:
                break;
        }
    }

#if true
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
#endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(1);
    }
}