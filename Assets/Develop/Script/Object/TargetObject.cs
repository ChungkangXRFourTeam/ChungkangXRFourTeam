using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour, IBActorLife
{
    [SerializeField] private float _hp;
    public bool IsDestroyed { get; private set; }
    public float MaxHp => _hp;
    private float _currentHp;
    public InteractionController Interaction { get; private set; }

    
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
    public void Die()
    {
        IsDestroyed = true;
        Destroy(gameObject);
    }

    public void DoHit(float damage)
    {
        CurrentHP -= damage;
    }

    private void Awake()
    {
        Interaction = GetComponentInChildren<InteractionController>();
        _currentHp = MaxHp;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<IBActorLife>(out var state))
        {
            DoHit(1);
        }

        if (other.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.Die();
        }
    }
}
