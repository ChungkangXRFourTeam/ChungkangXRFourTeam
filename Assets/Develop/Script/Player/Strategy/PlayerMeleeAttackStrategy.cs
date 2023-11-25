using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;
using XRProject.Helper;

[System.Serializable]
public class PlayerMeleeAttackStrategy : IStrategy
{
    private PlayerMeleeAttackData _data;
    private SpriteRenderer _renderer;
    private Transform _transform;

    private bool _isAttackPressed;

    public void Init(Blackboard blackboard)
    {
        _transform = blackboard.GetProperty<Transform>("out_transform");
        _renderer = _transform.gameObject.GetComponent<SpriteRenderer>();
        InputManager.RegisterActionToMainGame("Attack",OnKeyCallback,ActionType.Started);
        _data = blackboard.GetProperty<PlayerMeleeAttackData>("out_meleeAttackData");
    }
    private void Effect(Blackboard blackboard)
    {
        var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = _transform.position;
        pos.z = mp.z = 0f;
            
        var dir = mp - pos;
        bool flipX = dir.x > 0f ? true : false;

        var scale = _data.SlashEffectScale;
        scale.x *= flipX ? -1f : 1f;

        var farFromBody = dir.normalized;
        farFromBody.y = 0f;
        farFromBody.z = 0f;
        farFromBody.x *= _data.SlashEffectDistanceFromThis;
            
        EffectManager.ImmediateCommand(new EffectCommand()
        {
            EffectKey = "player/swordSlash",
            Position = _transform.position + (Vector3)_data.SlashEffectOffset + farFromBody,
            Scale = scale,
            OnContractActor = (x, y)=> OnEffectHit(x, blackboard, y),
            OnBeginCallback = x =>
            {
                DOTween.Sequence()
                    .SetDelay(Time.deltaTime * 10f)
                    .SetId(_sKey)
                    .OnComplete(() => x.Interaction.IsEnabled = false);
            }
        });
    }

    private void OnEffectHit(EffectItem item, Blackboard blackboard, ActorContractInfo info)
    {
        if (info.Transform.gameObject.CompareTag("Player")) return;
        
        if (
            blackboard.TryGetProperty<WrappedValue<int>>("out_remaingProperties", out var propertiesCount) &&
            blackboard.TryGetProperty<InteractionController>("out_interaction", out var myInteraction) &&
            myInteraction.TryGetContractInfo(out ActorContractInfo myInfo) &&
            myInfo.TryGetBehaviour(out IBActorProperties myProperties) &&
            info.TryGetBehaviour(out IBActorHit hit) &&
            info.TryGetBehaviour(out IBActorProperties properties))
        {
            blackboard.GetProperty("out_buffInfo", out BuffInfo buffInfo);
            float addingDamage = buffInfo.AddingDamage;
            
            if (propertiesCount > 0)
            {
                propertiesCount.Value -= 1;
                float damage = 1f * (properties.Properties == myProperties.Properties ? 1f : 2f) + addingDamage;
                hit.DoHit(myInteraction.ContractInfo, damage);
            }
            else
            {
                hit.DoHit(myInteraction.ContractInfo, 0.5f + addingDamage);
            }
        }
    }

    private object _sKey = new();
    private bool _canAttack = true;
    private int _attackCount = 0;

    private Blackboard _blackboard;

    public int CurrentAttackCount => _attackCount;
    
    public void Update(Blackboard blackboard)
    {
        _blackboard = blackboard;
        
        if(_isAttackPressed)
            OnAttack(blackboard);

        _isAttackPressed = false;
    }

    private bool CalcHitTime(ref float targetTime, float desTime)
    {
        targetTime += Time.deltaTime;

        if (targetTime >= desTime)
        {
            return true;
        }
        
        return false;
    }

    public void Reset()
    {
        InputManager.UnRegisterActionToMainGame("Attack",OnKeyCallback,ActionType.Started);
    }

    private void OnKeyCallback(InputAction.CallbackContext ctx)
    {
        _isAttackPressed = true;
    }

    void OnAttack(Blackboard blackboard)
    {
        blackboard.GetUnWrappedProperty<bool>("in_isGrabState", out var isGrabState);
        if (isGrabState) return;
        
        if (_canAttack)
        {
            _attackCount++;

            Effect(_blackboard);
            
            if (_attackCount >= _data.AttackMaxCount)
            {
                _canAttack = false;
                Sequence s = DOTween.Sequence();
                s.SetDelay(_data.AttackDelay).SetId(_sKey).OnComplete(() =>
                {
                    _canAttack = true;
                    _attackCount = 0;
                }).Play();
            }
        }
    }
}
