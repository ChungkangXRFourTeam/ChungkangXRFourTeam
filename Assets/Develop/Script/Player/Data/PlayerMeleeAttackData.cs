using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName="XR/Player/PlayerMeleeAttack", fileName="PlayerMeleeAttackData ", order = 3)]
public class PlayerMeleeAttackData : ScriptableObject
{
    [SerializeField] private float _distanceFromBodyforHand;
    [SerializeField] private Vector2 _slashEffectOffsetHit1;
    [SerializeField] private Vector2 _slashEffectOffsetHit2;
    [SerializeField] private Vector2 _slashEffectOffsetHit3;
    
    [SerializeField] private Vector2 _slashHitOffsetHit1;
    [SerializeField] private Vector2 _slashHitOffsetHit2;
    [SerializeField] private Vector2 _slashHitOffsetHit3;
    
    [SerializeField] private Vector2 _slashEffectScale;
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private int _attackMaxCount = 3;
    [SerializeField] private float slashHitRadiusHit1;
    [SerializeField] private float slashHitRadiusHit2;
    [SerializeField] private float slashHitRadiusHit3;

    public float DistanceFromBodyforHand => _distanceFromBodyforHand;

    public Vector2 SlashEffectOffsetHit1 => _slashEffectOffsetHit1;
    public Vector2 SlashEffectOffsetHit2 => _slashEffectOffsetHit2;
    public Vector2 SlashEffectOffsetHit3 => _slashEffectOffsetHit3;
    public Vector2 SlashHitOffsetHit1 => _slashHitOffsetHit1;
    public Vector2 SlashHitOffsetHit2 => _slashHitOffsetHit2;
    public Vector2 SlashHitOffsetHit3 => _slashHitOffsetHit3;

    public Vector2 SlashEffectScale => _slashEffectScale;

    public float AttackDelay => _attackDelay;
    public float SlashHitRadiusHit1 => slashHitRadiusHit1;
    public float SlashHitRadiusHit2 => slashHitRadiusHit2;
    public float SlashHitRadiusHit3 => slashHitRadiusHit3;

    public int AttackMaxCount => _attackMaxCount;
}
