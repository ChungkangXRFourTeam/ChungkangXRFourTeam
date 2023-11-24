using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private Rigidbody2D _rigid;
    
    
    private Vector2 GetMovingVector()
    {
        if (_inputLock) return Vector2.zero;

        return dir * _data.MovementSpeed;
    }
    
    
    public void Init(Blackboard blackboard)
    {
        blackboard.GetProperty("out_transform", out Transform transform);
        _prevPosition = transform.position;

        InputManager.RegisterActionToMainGame("Move",OnMove,ActionType.Performed);
        InputManager.RegisterActionToMainGame("Jump",OnJump,ActionType.Started);
        InputManager.RegisterActionToMainGame("Fall",OnFall,ActionType.Started);

        _data = blackboard.GetProperty<PlayerMoveData>("out_moveData");
        _rigid = blackboard.GetProperty<Rigidbody2D>("out_rigidbody");
    }

    private Vector2 _prevPosition;
    private Sequence _inputLockSequence;
    public void Update(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty("out_isGrounded", out bool isGrounded);
        if (_inputLock)
        {
            if (isGrounded)
            {
                if (_inputLockSequence != null)
                    return;
                
                _inputLockSequence = DOTween.Sequence();
                _inputLockSequence.SetDelay(0.35f).OnComplete(() =>
                {
                    _inputLock = false;
                    _inputLockSequence = null;
                });
            }

            return;
        }
        
        blackboard.GetProperty("out_buffInfo", out BuffInfo info);
        blackboard.GetProperty("out_transform", out Transform transform);
        blackboard.GetUnWrappedProperty("out_isLeftSide", out bool isLeftSide);
        blackboard.GetUnWrappedProperty("out_isRightSide", out bool isRightSide);
        blackboard.GetProperty("out_aniController", out PlayerAnimationController ani);
        
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
        if (fallingVector.sqrMagnitude > 0f && isGrounded == false)
        {
            blackboard.GetProperty("out_interaction", out InteractionController interaction);
            if (interaction.TryGetContractInfo(out ActorContractInfo aci) &&
                aci.TryGetBehaviour(out IBActorPhysics p))
            {
                p.Stop();
            }

            _inputLock = true;
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
