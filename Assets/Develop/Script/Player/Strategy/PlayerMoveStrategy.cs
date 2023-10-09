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
    
    private bool _inputLock;
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
    }
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
    }

    public void Reset()
    {
        
    }
}
