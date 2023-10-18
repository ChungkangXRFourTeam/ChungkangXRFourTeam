using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

[CreateAssetMenu(menuName="XR/Player/PlayerSwingAttack", fileName="PlayerSwingAttackData ", order = 3)]
public class PlayerSwingAttackData : ScriptableObject
{
    [SerializeField] private float _swingForce;
    [SerializeField] private float _minmumCloseDistance;
    [SerializeField] private float _timeScale;
    [SerializeField] private float _grabDistance;

    public float SwingForce => _swingForce;
    public float MinmumCloseDistance => _minmumCloseDistance;
    public float TimeScale => _timeScale;
    public float GrabDistance => _grabDistance;
}