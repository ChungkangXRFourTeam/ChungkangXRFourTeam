using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private List<PartOfBoss> _partList;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private float _coloringDuration;

    private void Awake()
    {
        _partList = new List<PartOfBoss>(GetComponentsInChildren<PartOfBoss>());

        foreach (var part in _partList)
        {
            part.Hit += Hit;
        }

        if (!_renderer)
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnDestroy()
    {
        foreach (var part in _partList)
        {
            part.Hit -= Hit;
        }
    }

    private void Hit()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(
            _renderer.material.DOColor(Color.red, _coloringDuration)
        );
        sequence.Append(
            _renderer.material.DOColor(Color.white, _coloringDuration)
        );

        sequence.Play();


    }
    
}
