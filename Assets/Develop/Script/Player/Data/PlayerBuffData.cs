using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="XR/Player/PlayerBuff", fileName="PlayerBuffData ", order = 3)]
public class PlayerBuffData : ScriptableObject
{
    [SerializeField] private float _flameBuffAddingDamage;
    [SerializeField] private float _waterBuffMovementSpeedFactor;

    public float FlameBuffAddingDamage => _flameBuffAddingDamage;
    public float WaterBuffMovementSpeedFactor => _waterBuffMovementSpeedFactor;
}