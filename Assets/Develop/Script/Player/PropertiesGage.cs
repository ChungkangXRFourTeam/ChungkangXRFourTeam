using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesGage : MonoBehaviour
{
    [SerializeField] private Image _image;
    private PlayerController _pc;
    private void Awake()
    {
        _pc = GameObject.Find("Player").GetComponent<PlayerController>();
        _image.fillAmount = 0f;
    }

    private void Update()
    {
        _image.fillAmount = _pc.RemainingPropertie / 10f;
        _image.color = _pc.Properties == EActorPropertiesType.Flame ? Color.red : Color.blue;
    }
}
