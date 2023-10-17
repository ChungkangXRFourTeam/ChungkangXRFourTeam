using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private Vector2 dir;
    [SerializeField] private Vector2 downDir;
    [SerializeField] private Vector2 upDir;
    
    private bool _inputLock;
    private EffectItem _effect;
    private Vector2 GetMovingVector()
    {
        if (_inputLock) return Vector2.zero;

        return dir * _movementSpeed;
    }
    
    
    public void Init(Blackboard blackboard)
    {
        blackboard.GetProperty("out_transform", out Transform transform);
        _effect = EffectManager.EffectItem("player/backTrail");
        
        if(_effect != null)
            _effect.EffectObject.transform.SetParent(transform);

        _prevPosition = transform.position;

        InputManager.ActionListener.MainGame.Move.performed += OnMove;
        InputManager.ActionListener.MainGame.Jump.started += OnJump;
        InputManager.ActionListener.MainGame.Fall.started += OnFall;

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
        var jumpingVector = upDir * _jumpForce;
        var fallingVector = downDir * _downForce;

        if (!isGrounded)
            jumpingVector = Vector2.zero;

        if (isLeftSide && movingVector.x < 0f)
            movingVector.x = 0f;
        if (isRightSide && movingVector.x > 0f)
            movingVector.x = 0f;
        
        _rigid.position += movingVector * Time.deltaTime;
        _rigid.AddForce(jumpingVector + fallingVector);

        downDir = Vector2.zero;
        upDir = Vector2.zero;
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

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (upDir == Vector2.zero)
            upDir = Vector2.up;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        dir = ctx.ReadValue<Vector2>();
    }

    void OnFall(InputAction.CallbackContext ctx)
    {
        if (downDir == Vector2.zero)
            downDir = Vector2.down;
    }
}
