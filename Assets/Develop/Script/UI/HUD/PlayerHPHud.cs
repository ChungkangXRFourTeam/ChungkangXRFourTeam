using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using XRProject.Utils.Log;

public class PlayerHPHud : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Color _color;
    [SerializeField] private float _animationDuration;
    
    private PlayerController _pc;

    private void Awake()
    {
        _pc = GameObject.Find("Player")?.GetComponent<PlayerController>();

        if (!_pc)
        {
            XLog.LogError("PlayerHPHud: it couldn't find player", "player");
            return;
        }
        if (!_image)
        {
            XLog.LogError("PlayerHPHud: image is null", "player");
            return;
        }

        _pc.ChangedHp += OnChangeHP;
    }

    private void OnValidate()
    {
        if (!_image) return;

        _image.color = _color;
    }

    private void OnChangeHP(IBActorLife life, float prevHp, float newHp)
    {
        _image.DOFillAmount( newHp / life.MaxHp, _animationDuration);
    }
}
