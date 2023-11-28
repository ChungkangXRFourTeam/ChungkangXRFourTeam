using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InteractionController))]
public class HealStation : MonoBehaviour, IBObjectHealStation
{
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _detectionRadius = 1f;
    [SerializeField] private float _coolTime = 20f;
    
    private InteractionController _interaction;
    private InputAction _action;
    private bool _canUse;
    public InteractionController Interaction => _interaction;

    private const string EFFECT_KEY = "actor/heal";
    
    private void Awake()
    {
        _canUse = true;
        _action = InputManager.GetMainGameAction("Interaction");
        
        _interaction = GetComponent<InteractionController>();
        
        _interaction.SetContractInfo(
            ObjectContractInfo.Create(transform, ()=>transform == false)
                .AddBehaivour<IBObjectHealStation>(this)
            );
    }

    private void Update()
    {
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        if (_action == null) return;
        var hit = Physics2D.OverlapCircle(transform.position, _detectionRadius, LayerMask.GetMask("Player"));
        if (hit == false) return;
        if (hit.TryGetComponent(out PlayerController pc))
        {
            if (_action.IsPressed())
            {
                Heal(pc.Interaction.ContractInfo as ActorContractInfo);
            }
        }
    }
    
    public void Heal(ActorContractInfo target)
    {
        if (_canUse == false) return;
        if (target == null) return;
        
        if (target.Transform.CompareTag("Player") &&
            target.TryGetBehaviour(out IBActorLife life))
        {
            life.CurrentHP = life.MaxHp;
            _canUse = false;
            DOTween.Sequence().SetDelay(_coolTime).OnComplete(() => _canUse = true);
            
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = EFFECT_KEY,
                Position = (Vector2)target.Transform.position + _offset
            });
        }
    }
}
