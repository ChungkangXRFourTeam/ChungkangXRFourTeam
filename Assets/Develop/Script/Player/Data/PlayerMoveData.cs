using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName="XR/Player/PlayerMove", fileName="PlayerMoveData", order = 3)]
public class PlayerMoveData : ScriptableObject
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _downForce;
    [SerializeField] private Vector2 _effectBackTrailOffset;
    [SerializeField] private float _effectBackTrailScaleFactor;

    public float MovementSpeed => _movementSpeed;

    public float JumpForce => _jumpForce;

    public float DownForce => _downForce;

    public Vector2 EffectBackTrailOffset => _effectBackTrailOffset;

    public float EffectBackTrailScaleFactor => _effectBackTrailScaleFactor;
}
