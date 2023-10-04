using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUi : MonoBehaviour
{
    [SerializeField] private GameObject actorState;
    [SerializeField] private Image _image;
    [SerializeField] private GameObject _canvas;

    private IBActorLife _life;
    private void Awake()
    {
        _canvas.SetActive(true);
        _life = actorState.GetComponent<IBActorLife>();
        if (_life != null)
        {
            _life.ChangedHp += OnChangedHp;
        }
    }

    private void OnChangedHp(IBActorLife actorLife, float prevHp, float currentHp)
    {
        _image.fillAmount = currentHp / actorLife.MaxHp;
    }
}
