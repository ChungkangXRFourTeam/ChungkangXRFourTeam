using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeableAnimationBodyEnemyAnimatorProperties : ChangeableAnimationBody
{
    public override bool IsEnabled { get; }

    [SerializeField] private Enemy _enemy;

    [SerializeField] private Animator _flame;
    [SerializeField] private Animator _water;

    private void Update()
    {
        if (_enemy)
        {
            SetAniFromProperties(_enemy.Properties);
        }
    }

    public override T GetComponentOrNull<T>()
    {
        if (typeof(T) != typeof(Animator)) return base.GetComponentOrNull<T>();

        if (_enemy.Properties == EActorPropertiesType.Flame)
        {
            return _flame as T;
        }
        else
        {
            return _water as T;
        }
    }

    private void SetAniFromProperties(EActorPropertiesType type)
    {
        bool isFlame = type == EActorPropertiesType.Flame;
        _flame.gameObject.SetActive(isFlame);
        _water.gameObject.SetActive(!isFlame);
    }

    public override void SetEnable(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }
}