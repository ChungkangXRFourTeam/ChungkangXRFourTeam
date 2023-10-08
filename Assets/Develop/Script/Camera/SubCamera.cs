using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    private Camera _cam;
    private CinemachineCameraControll _cameraController;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _cameraController = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineCameraControll>();
    }

    void Start()
    {
        _cameraController.RegisterCameraSizeChangeFunction(SetSubCameraOrthoSize);
    }
    public void SetSubCameraOrthoSize(float size)
    {
        _cam.orthographicSize = size;
    }
}
