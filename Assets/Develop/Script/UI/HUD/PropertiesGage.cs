using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using XRProject.Utils.Log;

public class PropertiesGage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private float _animationDuration;
    private PlayerController _pc;
    private EActorPropertiesType _prevType;

    private void SetAmount(EActorPropertiesType type, float value)
    {
        if (!_image) return;

        Color color = Color.white;
        
        if (type == EActorPropertiesType.Flame)
        {
            color = _flameColor;
        }        
        else if (type == EActorPropertiesType.Water)
        {
            color = _waterColor;
        }
        else
        {
            color = _emptyColor;
        }
        
        
        _image.DOFillAmount(value, _animationDuration);
        
        if(_prevType != type)   
            _image.DOColor(color, _animationDuration);
        
        _prevType = type;
        
    }
    private void Awake()
    {
        _pc = GameObject.Find("Player").GetComponent<PlayerController>();

        if (!_image)
        {
            XLog.LogError("PropertiesGage: image is null", "player");
            return;
        }
        
        if (!_pc)
        {
            XLog.LogError("PropertiesGage: it couldn't find player ", "player");
        }
        
        _image.fillAmount = 0f;
    }

    private void Update()
    {
        if (!_pc) return;
        SetAmount(_pc.Properties, _pc.RemainingPropertie / 10f);
        print(_pc.RemainingPropertie);
    }
}
