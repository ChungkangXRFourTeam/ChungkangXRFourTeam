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

        DOTween.Kill(this);
        _gageImage.DOFillAmount(value, _animationDuration).SetId(this);
        _headImage.DOFillAmount(value, _animationDuration).SetId(this);

        _gageImage.DOColor(color, _animationDuration).SetId(this);
        _headImage.DOColor(color, _animationDuration).SetId(this);
    }

    private void Awake()
    {
        _pc = GameObject.Find("Player").GetComponent<PlayerController>();

        if (!_gageImage || !_headImage)
        {
            XLog.LogError("PropertiesGage: image is null", "player");
            enabled = false;
            return;
        }

        if (!_pc)
        {
            XLog.LogError("PropertiesGage: it couldn't find player ", "player");
            enabled = false;
            return;
        }

        _gageImage.fillAmount = 0f;
        _gageImage.color = _emptyColor;
        _headImage.color = _emptyColor;

        _pc.ChangedProperties += OnChangeProperties;
    }

    private void OnDestroy()
    {
        _pc.ChangedProperties -= OnChangeProperties;
    }

    private void OnChangeProperties(EActorPropertiesType type)
    {
        SetAmount(type, _pc.RemainingPropertie / 10f);
    }
}