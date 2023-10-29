using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _followingSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _wideSize;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Vector2 _zoomOffset;
    [SerializeField] private Vector2 _centerPoint;
    [SerializeField] [Range(0f, 1f)] private float _outtingFactor;
    
    private Camera _camera;
    private Transform _target;
    private float _backupSize;
    
    private void Awake()
    {
        _target = GameObject.Find("Player").transform;
        
        _camera = GetComponent<Camera>();
        
        _backupSize = _camera.orthographicSize;
    }

    private bool _isZoomKeyDown;
    void Update()
    {
        if (!_target) return;
        
        Vector2 resultPos = Vector2.zero;

        var offset = _isZoomKeyDown ? _zoomOffset : _offset;
        if (_isZoomKeyDown)
        {
            Vector2 toTargetV = (_target.position - transform.position);
            float factor = toTargetV.magnitude * _outtingFactor;
            resultPos = _centerPoint + offset + toTargetV.normalized * factor;
        }
        else
        {
            resultPos = (Vector2)_target.position + offset;
        }
        
        
        Vector3 pos = Vector2.Lerp(transform.position, resultPos, _followingSpeed * Time.unscaledDeltaTime);
        
        pos.z = -10f;
        transform.position = pos; 
        
        if (_isZoomKeyDown)
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _wideSize, _zoomSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _backupSize, _zoomSpeed * Time.unscaledDeltaTime);
        }
    }
    
}
