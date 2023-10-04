using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartOfBoss : MonoBehaviour, IBActorLife, IBActorHit
{
    [SerializeField] private InteractionController _interactive;
    [SerializeField] private float _hp;
    public float MaxHp => _hp;
    private float _currentHp;
    
    public float CurrentHP
    {
        get => _currentHp;
        set
        {
            float backup = _currentHp;
            _currentHp = value;
            ChangedHp?.Invoke(this, backup, _currentHp);

            if (_currentHp <= 0f)
            {
                Die();
            }
        }
    }

    public event Action<IBActorLife, float, float> ChangedHp;
    public event Action Hit;

    public InteractionController Interaction { get; private set; }

    private void Awake()
    {
        Interaction = GetComponentInChildren<InteractionController>();
        _currentHp = MaxHp;
        _interactive.SetContractInfo(ActorContractInfo.Create(transform, () => IsDestroyed)
            .AddBehaivour<IBActorHit>(this)
            .AddBehaivour<IBActorLife>(this)
            );
        _interactive.OnContractActor += OnContractActor;
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if (info.TryGetBehaviour(out IBActorHit hit))
        {
            hit.DoHit(_interactive.ContractInfo,1);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public bool IsDestroyed { get; } = false;
    public void DoHit(BaseContractInfo caller, float damage)
    {
        Hit?.Invoke();
        CurrentHP -= damage;
    }
}
