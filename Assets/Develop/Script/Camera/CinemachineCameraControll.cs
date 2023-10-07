using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraControll : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CinemachineFramingTransposer _framingTransposer;
    [SerializeField] private CinemachineFollowZoom _followZoom;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _leftWall;
    [SerializeField] private Transform _rightWall;
    [SerializeField] private Transform _upWall;
    [SerializeField] private Transform _downWall;
    [Space(15f), Header("카메라 줌")]
    [SerializeField, Tooltip("최대 줌 아웃 사이즈를 조절합니다.")] private float _maxZoomOutSize = 10f;
    [SerializeField, Tooltip("최대 줌 인 사이즈를 조절합니다.")] private float _maxZoomInSize = 15f;
    [SerializeField, Tooltip("카메라가 줌 되는 속도를 조절합니다.")] private float _zoomSpeed = 1f;

    private bool _isZoomKeyDown;
    // Start is called before the first frame update
    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _camera = Camera.main;
        _leftWall = GameObject.Find("LeftWall").transform;
        _rightWall = GameObject.Find("RightWall").transform;
        _upWall = GameObject.Find("UpWall").transform;
        _downWall = GameObject.Find("DownWall").transform;

        _followZoom = GetComponent<CinemachineFollowZoom>();
        _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _isZoomKeyDown = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isZoomKeyDown = false;
        }

        if (!_camera.orthographic)
        {
            ZoomWithPerspective();
        }
        else
        {
            ZoomWithOrthographic();
        }
    }

    void ZoomWithPerspective()
    {

        if (_isZoomKeyDown)
        {
            _followZoom.m_MinFOV = Mathf.Lerp(_followZoom.m_MinFOV, 120f, Time.deltaTime);
            
            if (_camera.WorldToViewportPoint(_leftWall.position).x > 0.01f)
            {
                _framingTransposer.m_TrackedObjectOffset.x += 0.05f;
            }
            if (_camera.WorldToViewportPoint(_rightWall.position).x < 0.99f)
            {
                _framingTransposer.m_TrackedObjectOffset.x -= 0.05f;
            }
            if (_camera.WorldToViewportPoint(_upWall.position).y < 0.96f)
            {
                _framingTransposer.m_TrackedObjectOffset.y -= 0.05f;
            }
            if (_camera.WorldToViewportPoint(_downWall.position).y > 0.03f)
            {
                _framingTransposer.m_TrackedObjectOffset.y += 0.05f;
            }
        }
        else
        {
            _framingTransposer.m_TrackedObjectOffset.y = Mathf.Lerp(_framingTransposer.m_TrackedObjectOffset.y, 0f, Time.deltaTime);
            _framingTransposer.m_TrackedObjectOffset.x = Mathf.Lerp(_framingTransposer.m_TrackedObjectOffset.x, 0f, Time.deltaTime);
            _followZoom.m_MinFOV = Mathf.Lerp(_followZoom.m_MinFOV, 90f, Time.deltaTime * _zoomSpeed);
        }
    }

    void ZoomWithOrthographic()
    {
        if (_isZoomKeyDown)
        {
            _virtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(_virtualCamera.m_Lens.OrthographicSize, _maxZoomOutSize, Time.deltaTime * _zoomSpeed);
        }
        else
        {
            _virtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(_virtualCamera.m_Lens.OrthographicSize, _maxZoomInSize, Time.deltaTime * _zoomSpeed);
        }
    }
}
