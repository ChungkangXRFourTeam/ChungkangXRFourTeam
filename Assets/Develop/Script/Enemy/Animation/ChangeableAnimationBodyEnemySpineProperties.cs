using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ChangeableAnimationBodyEnemySpineProperties : ChangeableAnimationBody
{
    public override bool IsEnabled { get; }

    [SerializeField] private Enemy _enemy;
    [SerializeField] private SkeletonAnimation _ani;

    private void Update()
    {
        if (_enemy)
        {
            SetAniFromProperties(_enemy.Properties);
        }
    }
    private void SetAniFromProperties(EActorPropertiesType type)
    {
        bool isFlame = type == EActorPropertiesType.Flame;

        if (isFlame)
        {
            _ani.skeleton.SetSkin("Orange");
        }
        else
        {
            _ani.skeleton.SetSkin("Blue");
        }
    }

    public override void SetEnable(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }
}