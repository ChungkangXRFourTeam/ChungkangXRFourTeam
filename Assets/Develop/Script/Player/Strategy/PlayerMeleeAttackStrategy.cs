using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

[System.Serializable]
public class PlayerMeleeAttackStrategy : IStrategy
{
    [SerializeField] private Transform _hand;
    [SerializeField] private float _distanceFromBodyforHand;
    [SerializeField] private float _slashEffectDistanceFromThis;
    [SerializeField] private Vector2 _slashEffectOffset;
    [SerializeField] private Vector2 _slashEffectScale;

    private Transform transform;

    public void Init(Blackboard blackboard)
    {
        transform = blackboard.GetProperty<Transform>("out_transform");
    }
    private void Attack(Blackboard blackboard)
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var col = Physics2D.OverlapCircle(_hand.position, _hand.lossyScale.x, LayerMask.GetMask("Enemy"));
        if (!col) return;

        var dir = ((Vector2)_hand.position - (Vector2)transform.position).normalized;
        
        if (
            blackboard.TryGetProperty<WrappedValue<int>>("out_remaingProperties", out var propertiesCount) &&
            blackboard.TryGetProperty<InteractionController>("out_interaction", out var myInteraction) &&
            myInteraction.TryGetContractInfo(out ActorContractInfo myInfo) &&
            myInfo.TryGetBehaviour(out IBActorProperties myProperties) &&
            col.TryGetComponent(out InteractionController com) &&
            com.TryGetContractInfo(out ActorContractInfo info) && 
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

    private void RotateHand()
    {
        var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = transform.position;
        pos.z = mp.z = 0f;
        var dir = mp - pos;
        dir = Vector3.Project(dir, Vector3.right).normalized;


        _hand.position = transform.position + dir * _distanceFromBodyforHand;
    }

    public void Update(Blackboard blackboard)
    {
        RotateHand();
        Attack(blackboard);
        
        if (Input.GetMouseButtonDown(0))
        {
            var mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = transform.position;
            pos.z = mp.z = 0f;
            
            var dir = mp - pos;
            bool flipX = dir.x > 0f ? true : false;

            var scale = _slashEffectScale;
            scale.x *= flipX ? -1f : 1f;

            var farFromBody = dir.normalized;
            farFromBody.y = 0f;
            farFromBody.z = 0f;
            farFromBody.x *= _slashEffectDistanceFromThis;
            
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "player/swordSlash",
                Position = transform.position + (Vector3)_slashEffectOffset + farFromBody,
                Scale = scale,
            });
        }
    }

    public void Reset()
    {
        
    }
}
