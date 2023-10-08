using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraControll : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [Space(15f), Header("카메라 줌")]
    [SerializeField, Tooltip("최대 줌 아웃 사이즈를 조절합니다.")] private float _maxZoomOutSize = 10f;
    [SerializeField, Tooltip("최대 줌 인 사이즈를 조절합니다.")] private float _maxZoomInSize = 15f;
    [SerializeField, Tooltip("카메라가 줌 되는 속도를 조절합니다.")] private float _zoomSpeed = 1f;

    public delegate void SetCameraOrthoSize(float size);
    private SetCameraOrthoSize onMainCameraSizeChanged;
    private bool _isZoomKeyDown;
    
    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    { 
        ZoomCamera();
    }


    private void ZoomCamera()
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
        
        onMainCameraSizeChanged(_virtualCamera.m_Lens.OrthographicSize);
    }

    public void RegisterCameraSizeChangeFunction(SetCameraOrthoSize subCameraSizeChangeFunction)
    {
        onMainCameraSizeChanged += subCameraSizeChangeFunction;
    }

    public void SetZoomKeyState(bool state)
    {
        _isZoomKeyDown = state;
    }

}
