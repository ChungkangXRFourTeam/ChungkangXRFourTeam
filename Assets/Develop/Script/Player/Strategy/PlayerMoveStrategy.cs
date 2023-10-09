using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;



[Serializable]
public class PlayerMoveStrategy : IStrategy
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _downForce;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private Vector2 _effectBackTrailOffset;
    [SerializeField] private float _effectBackTrailScaleFactor;
    
    private bool _inputLock;
    private EffectItem _effect;
    private Vector2 GetMovingVector()
    {
        if (_inputLock) return Vector2.zero;

        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
        }

        return dir * _movementSpeed;
    }

    private Vector2 GetJumpingVector()
    {
        if (_inputLock) return Vector2.zero;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            return Vector2.up * _jumpForce;
        }
        
        return Vector2.zero;
    }

    private Vector2 GetFallingVector()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {            
            return Vector2.down * _downForce;
        }

        return Vector2.zero;
    }
    
    
    public void Init(Blackboard blackboard)
    {
        blackboard.GetProperty("out_transform", out Transform transform);
        _effect = EffectManager.EffectItem("player/backTrail");
        
        if(_effect != null)
            _effect.EffectObject.transform.SetParent(transform);

        _prevPosition = transform.position;
    }

    private Vector2 _prevPosition;
    public void Update(Blackboard blackboard)
    {
        blackboard.GetProperty("out_buffInfo", out BuffInfo info);
        blackboard.GetProperty("out_transform", out Transform transform);
        blackboard.GetUnWrappedProperty("out_isGrounded", out bool isGrounded);
        blackboard.GetUnWrappedProperty("out_isLeftSide", out bool isLeftSide);
        blackboard.GetUnWrappedProperty("out_isRightSide", out bool isRightSide);

        var movingVector = GetMovingVector() * Mathf.Max(info.SpeedFactor, 1f);
        var jumpingVector = GetJumpingVector();
        var fallingVector = GetFallingVector();

        if (!isGrounded)
            jumpingVector = Vector2.zero;

        if (isLeftSide && movingVector.x < 0f)
            movingVector.x = 0f;
        if (isRightSide && movingVector.x > 0f)
            movingVector.x = 0f;
        
        _rigid.position += movingVector * Time.deltaTime;
        _rigid.AddForce(jumpingVector + fallingVector);
        
        if (fallingVector.sqrMagnitude > 0f)
        {
            _rigid.velocity = Vector2.zero;
            
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "player/airDownDash",
                Position = transform.position,
                Rotation = Quaternion.Euler(0f, 0f, 180f),
                Scale = Vector3.one * 2.5f,
                FlipRotation = 1f
            });
        }
        if (jumpingVector.sqrMagnitude > 0f)
        {
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "player/jumpDust",
                Position = transform.position,
                Rotation = Quaternion.Euler(0f, 0f, 0f),
                Scale = Vector3.one * 1.5f
            });
        }

        if (movingVector.sqrMagnitude > 0f)
        {
            if (_effect != null)
            {
                var dir = (Vector2)transform.position - _prevPosition;
                dir = dir.normalized;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                
                _effect.IsEnabled = true;
                _effect.ApplyCommand(new EffectCommand()
                {
                    Position = transform.position,
                    Scale = Vector3.one * _effectBackTrailScaleFactor,
                    Rotation = Quaternion.Euler(0f, 0f, 180 + angle)
                });
            }
        }
        else
        {
            if (_effect != null)
            {
                _effect.IsEnabled = false;
            }
        }

        _prevPosition = transform.position;
    }

    public void Reset()
    {
        
    }
}
