using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionController))]
public class HealStation : MonoBehaviour, IBObjectHealStation
{
    [SerializeField] private Vector2 _offset;
    
    private InteractionController _interaction;
    public InteractionController Interaction => _interaction;

    private const string EFFECT_KEY = "actor/heal";
    
    private void Awake()
    {
        _interaction = GetComponent<InteractionController>();
        
        _interaction.SetContractInfo(
            ObjectContractInfo.Create(transform, ()=>transform == false)
                .AddBehaivour<IBObjectHealStation>(this)
            );

        _interaction.OnContractActor += OnContractActor;
    }

    private void OnContractActor(ActorContractInfo info)
    {
        Heal(info);
    }

    public void Heal(ActorContractInfo target)
    {
        if (target.Transform.CompareTag("Player") &&
            target.TryGetBehaviour(out IBActorLife life))
        {
            life.CurrentHP = life.MaxHp;
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = EFFECT_KEY,
                Position = (Vector2)target.Transform.position + _offset
            });
        }
    }
}
