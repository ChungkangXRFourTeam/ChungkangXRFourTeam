using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private SkeletonAnimation _ani;
    private void Awake()
    {
        _ani = GetComponent<SkeletonAnimation>();

        _ani.AnimationName = "Idle";
    }

}
