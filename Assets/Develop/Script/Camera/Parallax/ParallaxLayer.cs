using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum LayerType
{
    [InspectorName("기본값")]
    Default,
    [InspectorName("원경")]
    Background,
    [InspectorName("중경")]
    Middleground,
    [InspectorName("근경")]
    Foreground
}

[AddComponentMenu("패럴랙스 스크롤/패럴랙스 레이어"), ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Header("오브젝트 타입 설정")]
    [SerializeField, Tooltip("오브젝트의 타입을 정합니다.")]
    private LayerType _layerType;
    [Header("오브젝트 x축 속도 설정")]
    [SerializeField, Tooltip("오브젝트의 수평 이동 속도를 조절합니다."), Range(-2f, 2f)] private float _horizontalFactor;
    [Header("오브젝트 y축 속도 설정")]
    [SerializeField, Tooltip("오브젝트의 수직 이동 속도를 조절합니다."),Range(-1f, 1f)] private float _verticalFactor;
    [Header("오브젝트 크기 설정")]
    [SerializeField, Tooltip("스프라이트의 사이즈를 조절합니다."), Range(-2f, 3f)]
    private float _spriteSize;
    private SpriteRenderer _renderer;
    
    [Space(20f)]
    [SerializeField]
    private Transform _leftPoint;
    [SerializeField]
    private Transform _rightPoint;
    [SerializeField]
    private Transform _upPoint;
    [SerializeField]
    private Transform _downPoint;

    private Vector2 _leftWall;
    private Vector2 _rightWall;
    private Vector2 _upWall;
    private Vector2 _downWall;
    

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        float posX = transform.position.x;
        float posY = transform.position.y;
        _leftPoint.position = new Vector2((posX - (_renderer.size.x / 2) * transform.localScale.x),posY);
        _rightPoint.position = new Vector2((posX + (_renderer.size.x / 2) * transform.localScale.x),posY);
        _upPoint.position = new Vector2(posX,posY+ (_renderer.size.y / 2) * transform.localScale.y);
        _downPoint.position = new Vector2(posX,posY - (_renderer.size.y / 2)*transform.localScale.y);

        _leftWall = GameObject.Find("LeftWall").transform.position;
        _rightWall = GameObject.Find("RightWall").transform.position;
        _upWall = GameObject.Find("UpWall").transform.position;
        _downWall= GameObject.Find("DownWall").transform.position;
        SetLayer();
    }

    void SetLayer()
    {
        switch (_layerType)
        {
            case LayerType.Default:
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                _renderer.sortingLayerName = "Default";
                break;
            case LayerType.Background:
                gameObject.layer = LayerMask.NameToLayer("Background");
                gameObject.tag = "Background";
                _renderer.sortingLayerName = "Background";
                break;
            case LayerType.Middleground:
                gameObject.layer = LayerMask.NameToLayer("Middleground");
                _renderer.sortingLayerName = "Middleground";
                break;
            case LayerType.Foreground:
                gameObject.layer = LayerMask.NameToLayer("Foreground");
                _renderer.sortingLayerName = "Foreground";
                break;
        }
    }
    public void MoveX(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * _horizontalFactor;
        transform.position = newPos;
    }
    
    public void MoveY(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= delta * _verticalFactor;
        transform.position = newPos;
    }

    private void Update()
    {
        if (transform.localScale.x != _spriteSize)
        {
            transform.localScale = new Vector2(_spriteSize, _spriteSize);
        }
        
        SetLayer();
    }
}