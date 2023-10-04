using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorObject : MonoBehaviour
{
    [SerializeField] private bool _awakeOpen;
    [SerializeField] private Transform _openPivot;
    
    //private bool _isOpend;
    private Vector2 _closePos;

    private void Awake()
    {
        _closePos = transform.position;
        _openPivot.gameObject.SetActive(false);
        
        if (_awakeOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public void Open()
    {
        transform.position = (Vector2)_openPivot.position;
        //_isOpend = false;
    }

    public void Close()
    {
        transform.position = _closePos;
        //_isOpend = false;
    }
}
