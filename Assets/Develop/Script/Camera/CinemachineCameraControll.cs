using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraControll : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [Space(15f), Header("카메라 줌아웃")]
    [SerializeField, Tooltip("최대 줌 아웃 사이즈를 조절합니다.")] private float _maxZoomOutSize = 10f;
    [Space(15f), Header("카메라 줌인")]
    [SerializeField, Tooltip("최대 줌 인 사이즈를 조절합니다.")] private float _maxZoomInSize = 15f;
    [Space(15f), Header("카메라 줌 속도")]
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
            // Time.deltaTIme => Time.unscaledDeltaTime 으로 수정: 손형준, 23/10/10-19:24
            _virtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(_virtualCamera.m_Lens.OrthographicSize, _maxZoomOutSize, Time.unscaledDeltaTime * _zoomSpeed);
        }
        else
        {
            // Time.deltaTIme => Time.unscaledDeltaTime 으로 수정: 손형준, 23/10/10-19:24
            _virtualCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(_virtualCamera.m_Lens.OrthographicSize, _maxZoomInSize, Time.unscaledDeltaTime * _zoomSpeed);
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

    public float GetMaxZoomOutSize()
    {
        return _maxZoomOutSize;
    }

    public float GetMaxZoomInSize()
    {
        return _maxZoomInSize;
    }

}
