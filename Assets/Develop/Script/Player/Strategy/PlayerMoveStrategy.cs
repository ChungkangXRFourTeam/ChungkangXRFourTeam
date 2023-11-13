using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XRProject.Helper;



[Serializable]
public class PlayerMoveStrategy : IStrategy
{
    private PlayerMoveData _data;
    private Vector2 dir;
    private Vector2 downDir;
    private Vector2 upDir;
    private bool _inputLock;
    private EffectItem _effect;
    private Rigidbody2D _rigid;
    
    private Vector2 GetMovingVector()
    {
        if (_inputLock) return Vector2.zero;

        return dir * _data.MovementSpeed;
    }
    
    
    public void Init(Blackboard blackboard)
    {
        blackboard.GetProperty("out_transform", out Transform transform);
        _effect = EffectManager.EffectItem("player/backTrail");
        
        if(_effect != null)
            _effect.EffectObject.transform.SetParent(transform);

        _prevPosition = transform.position;

        InputManager.RegisterActionToMainGame("Move",OnMove,ActionType.Performed);
        InputManager.RegisterActionToMainGame("Jump",OnJump,ActionType.Started);
        InputManager.RegisterActionToMainGame("Fall",OnFall,ActionType.Started);

        _data = blackboard.GetProperty<PlayerMoveData>("out_moveData");
        _rigid = blackboard.GetProperty<Rigidbody2D>("out_rigidbody");
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
        var jumpingVector = upDir * _data.JumpForce;
        var fallingVector = downDir * _data.DownForce;

        if (!isGrounded)
            jumpingVector = Vector2.zero;

        if (isLeftSide && movingVector.x < 0f)
            movingVector.x = 0f;
        if (isRightSide && movingVector.x > 0f)
            movingVector.x = 0f;
        
        _rigid.position += movingVector * Time.deltaTime;
        _rigid.AddForce(jumpingVector, ForceMode2D.Impulse);

        downDir = Vector2.zero;
        upDir = Vector2.zero;
        if (fallingVector.sqrMagnitude > 0f)
        {
            blackboard.GetProperty("out_interaction", out InteractionController interaction);
            if (interaction.TryGetContractInfo(out ActorContractInfo aci) &&
                aci.TryGetBehaviour(out IBActorPhysics p))
            {
                p.Stop();
            }
            
            _rigid.velocity = Vector2.zero;
            _rigid.AddForce(fallingVector, ForceMode2D.Impulse);
            
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
                    Scale = Vector3.one * _data.EffectBackTrailScaleFactor,
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
