using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum LayerType
{
    [InspectorName("기본값")]
    Default,
    [InspectorName("원경")]
    Background,
    [InspectorName("오브젝트1")]
    Object1,
    [InspectorName("오브젝트2")]
    Object2,
    [InspectorName("근경")]
    Foreground
}

[AddComponentMenu("패럴랙스 스크롤/패럴랙스 레이어")]
[ExecuteInEditMode]
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
    private float _spriteSize = 1f;
    
    [Header("카메라 줌아웃 할때 크기를 변경 할 것인지에 대한 여부")]
    [SerializeField, Tooltip("줌아웃 했을 때 오브젝트의 크기를 변경 할 것인지에 대한 여부를 설정합니다.")]
    private bool allowChangeSizeWhenCameraSizeChanged;
    
    [Header("오브젝트 줌아웃 크기 설정")]
    [SerializeField, Tooltip("줌아웃 했을 때의 스프라이트 크기를 조절합니다."), Range(-2f, 3f)]
    private float _spriteZoomOutSize = 1f;
    private SpriteRenderer _renderer;
    private CinemachineCameraControll _virtualCameraController;
    private CinemachineVirtualCamera _virtualCamera;

    [Header("벽을 넘어갈 수 있는지에 대한 여부")] [SerializeField]
    private bool isOverable;
    
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
        _virtualCameraController = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineCameraControll>();
        _virtualCamera = GameObject.FindWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();

    }

    private void Start()
    {
        float posX = transform.position.x;
        float posY = transform.position.y;
        SetLayer();
        if (_renderer.sprite != null)
        {
            float sizeX = _renderer.sprite.texture.Size().x / 100;
            float sizeY = _renderer.sprite.texture.Size().y / 100;
            
            _leftPoint.position = new Vector2((posX - (sizeX / 2) * transform.localScale.x), posY);
            _rightPoint.position = new Vector2((posX + (sizeX  / 2) * transform.localScale.x), posY);
            _upPoint.position = new Vector2(posX, posY + (sizeY / 2) * transform.localScale.y);
            _downPoint.position = new Vector2(posX, posY - (sizeY / 2) * transform.localScale.y);
        }

        _leftWall = GameObject.Find("LeftWall").transform.position;
        _rightWall = GameObject.Find("RightWall").transform.position;
        _upWall = GameObject.Find("UpWall").transform.position;
        _downWall= GameObject.Find("DownWall").transform.position;
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
            case LayerType.Object1:
                gameObject.layer = LayerMask.NameToLayer("Object1");
                gameObject.tag = "Object1";
                _renderer.sortingLayerName = "Object1";
                break;
            case LayerType.Object2:
                gameObject.layer = LayerMask.NameToLayer("Object2");
                gameObject.tag = "Object2";
                _renderer.sortingLayerName = "Object2";
                break;
            case LayerType.Foreground:
                gameObject.layer = LayerMask.NameToLayer("Foreground");
                gameObject.tag = "Foreground";
                _renderer.sortingLayerName = "Foreground";
                break;
        }
    }
    public void MoveX(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * _horizontalFactor;

        if (isOverable||(_leftPoint.position.x > _leftWall.x && delta > 0) || (_rightPoint.position.x < _rightWall.x && delta < 0))
        {
            transform.position = newPos;
        }
    }
    
    public void MoveY(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= delta * _verticalFactor;
        
        if(isOverable||((_upPoint.position.y < _upWall.y && delta < 0) || (_downPoint.position.y > _downWall.y && delta > 0))
           ) 
            transform.position = newPos;
    }

    private void Update()
    {
        SetSpriteSize();
    }

    void SetSpriteSize()
    {
            InputAction action = InputManager.GetMainGameAction("Grab");
            if (action != null && action.IsPressed())
            {
                if (allowChangeSizeWhenCameraSizeChanged && Application.isPlaying)
                {
                    transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(_spriteZoomOutSize, _spriteZoomOutSize),Time.unscaledDeltaTime * 5);
                }
            }
            else
            {
                transform.localScale = Vector2.Lerp(transform.localScale,new Vector2(_spriteSize, _spriteSize),Time.unscaledDeltaTime * 5);
            }

    }

}