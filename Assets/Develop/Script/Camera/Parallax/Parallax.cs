using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
public class Parallax : MonoBehaviour
{
    private GameObject _player;
    private GameObject _virtualCameraObject;
    private CinemachineVirtualCamera _virtualCamera;
    private ParallaxCamera _parallaxCamera;
    [SerializeField] private GameObject[] _parallaxLayers;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _virtualCameraObject = GameObject.FindGameObjectWithTag("VirtualCamera");
    }

    void Start()
    {
        if (_virtualCameraObject != null && _virtualCamera == null)
        {
            _virtualCamera = _virtualCameraObject.GetComponent<CinemachineVirtualCamera>();
            _virtualCamera.Follow = _player.transform;
        }
        
        if (_parallaxCamera == null)
            _parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        if (_parallaxCamera != null)
            _parallaxCamera.onCameraTranslate += Move;

        _parallaxLayers = GameObject.FindGameObjectsWithTag("Background");
    }
    

    void Move(float deltaX, float deltaY)
    {
        for (int i = 0; i < _parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = _parallaxLayers[i].GetComponent<ParallaxLayer>();
            layer.MoveX(deltaX);
            layer.MoveY(deltaY);
        }
    }

}
