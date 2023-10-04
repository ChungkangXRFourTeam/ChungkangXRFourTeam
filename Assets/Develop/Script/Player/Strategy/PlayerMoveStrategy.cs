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

        //if (Input.GetKey(KeyCode.W))
        //{
        //    dir += Vector2.up;
        //}
//
        //if (Input.GetKey(KeyCode.S))
        //{
        //    dir += Vector2.down;
        //}

        return dir * _movementSpeed;
    }

    private Vector2 GetJumpingVector()
    {
        if (_inputLock) return Vector2.zero;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            return (GameSetting.Instance.IsGravityDown ? Vector2.up : Vector2.down) * _jumpForce;
        }
        
        return Vector2.zero;
    }
    
    
    public void Init(Blackboard blackboard)
    {
    }
    public void Update(Blackboard blackboard)
    {
        var movingVector = GetMovingVector();
        var jumpingVector = GetJumpingVector();

        _rigid.position += movingVector * Time.deltaTime;
        _rigid.AddForce(jumpingVector);

        if (Input.GetKeyDown(KeyCode.F))
        {
            GameSetting.Instance.IsGravityDown = !GameSetting.Instance.IsGravityDown;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _rigid.velocity = Vector2.zero;
            _rigid.AddForce(Vector2.down * _downForce);
        }
    }

    public void Reset()
    {
        
    }
}
