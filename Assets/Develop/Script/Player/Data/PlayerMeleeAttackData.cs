using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="XR/Player/PlayerMeleeAttack", fileName="PlayerMeleeAttackData ", order = 3)]
public class PlayerMeleeAttackData : ScriptableObject
{
    [SerializeField] private float _distanceFromBodyforHand;
    [SerializeField] private float _slashEffectDistanceFromThis;
    [SerializeField] private Vector2 _slashEffectOffset;
    [SerializeField] private Vector2 _slashEffectScale;
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private int _attackMaxCount = 3;

    public float DistanceFromBodyforHand => _distanceFromBodyforHand;

    public float SlashEffectDistanceFromThis => _slashEffectDistanceFromThis;

    public Vector2 SlashEffectOffset => _slashEffectOffset;

    public Vector2 SlashEffectScale => _slashEffectScale;

    public float AttackDelay => _attackDelay;

    public int AttackMaxCount => _attackMaxCount;
}
