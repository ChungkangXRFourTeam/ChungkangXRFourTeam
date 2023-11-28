using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HpUi : MonoBehaviour
{
    [SerializeField] private GameObject actorState;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private float _duration;

    private IBActorLife _life;
    private void Awake()
    {
        _life = actorState.GetComponent<IBActorLife>();
        if (_life != null)
        {
            _life.ChangedHp += OnChangedHp;
        }
    }

    private void OnChangedHp(IBActorLife actorLife, float prevHp, float currentHp)
    {
        DOTween.Kill(this);
        _image.DOFillAmount(currentHp / actorLife.MaxHp, _duration).SetId(this);
    }
}
