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
    [SerializeField] private GameObject[] _parallaxLayerObjects;
    [SerializeField] private List<ParallaxLayer> _parallaxLayers;

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

        _parallaxLayerObjects = GameObject.FindGameObjectsWithTag("Background");
        if (_parallaxLayerObjects != null)
        {
            _parallaxLayers = new List<ParallaxLayer>();
            for (int i = 0; i < _parallaxLayerObjects.Length; i++)
            {
                _parallaxLayers.Add( _parallaxLayerObjects[i].GetComponent<ParallaxLayer>());
            }
        }
    }
    

    void Move(float deltaX, float deltaY)
    {
        for (int i = 0; i < _parallaxLayers.Count; i++)
        {
            _parallaxLayers[i].MoveX(deltaX);
            _parallaxLayers[i].MoveY(deltaY);
        }
    }

}
