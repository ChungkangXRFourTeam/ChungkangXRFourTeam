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
    [SerializeField] private Image _gageImage;
    [SerializeField] private Image _headImage;
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Color _emptyColor;
    [SerializeField] private float _animationDuration;
    private PlayerController _pc;
    private EActorPropertiesType _prevType;
    private float _prevValue;

    private void SetAmount(EActorPropertiesType type, float value)
    {
        if (!_gageImage) return;

        Color color = _emptyColor;

        if (type == EActorPropertiesType.Flame)
        {
            color = _flameColor;
        }
        else if (type == EActorPropertiesType.Water)
        {
            color = _waterColor;
        }

        if (!Mathf.Approximately(_prevValue, value))
        {
            _gageImage.DOFillAmount(value, _animationDuration);
            _headImage.DOFillAmount(value, _animationDuration);
        }

        if (_prevType != type)
        {
            _gageImage.DOColor(color, _animationDuration);
            _headImage.DOColor(color, _animationDuration);
        }

        _prevType = type;
        _prevValue = value;
    }

    private void Awake()
    {
        _pc = GameObject.Find("Player").GetComponent<PlayerController>();

        if (!_gageImage || !_headImage)
        {
            XLog.LogError("PropertiesGage: image is null", "player");
            return;
        }

        if (!_pc)
        {
            XLog.LogError("PropertiesGage: it couldn't find player ", "player");
        }

        _gageImage.fillAmount = 0f;
        _gageImage.color = _emptyColor;
        _headImage.color = _emptyColor;
    }

    private void Update()
    {
        if (!_pc) return;
        SetAmount(_pc.Properties, _pc.RemainingPropertie / 10f);
    }
}