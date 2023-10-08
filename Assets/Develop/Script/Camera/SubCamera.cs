using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCamera : MonoBehaviour
{
    private float mainCamOrthoSize;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        mainCamOrthoSize = Camera.main.orthographicSize;
        cam.orthographicSize = mainCamOrthoSize;
    }
}
