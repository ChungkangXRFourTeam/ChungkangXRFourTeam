using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="XR/Enemy/EnemyData", fileName="EnemyData ", order = 3)]
public class EnemyData : ScriptableObject
{

    [SerializeField] private float _hp;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _SleepDecisionTime;
    [SerializeField] private float _propagationForce;
    [SerializeField] private int _propagationCount;
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private float _detectionDistance;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _hedgehogAttackDuration;
    public float SleepDecisionTime => _SleepDecisionTime;   
    public float PropagationForce => _propagationForce;


    public float Hp => _hp;
    public Color FlameColor => _flameColor;

    public Color WaterColor => _waterColor;

    public float MovementSpeed => _movementSpeed;
    public int PropagationCount => _propagationCount;
    public float DetectionDistance => _detectionDistance;

    public float AttackDistance => _attackDistance;
    public float Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public float HedgehogAttackDuration => _hedgehogAttackDuration;
}
