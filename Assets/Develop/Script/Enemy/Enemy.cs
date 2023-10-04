using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : ActorPhysics, IBActorLife, IBActorProperties, IBActorHit, IBActorThrowable
{
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Image _effectImage;
    [SerializeField] private float _hp;

    public event System.Action<IBActorLife, float, float> ChangedHp;
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

    [SerializeField]
    private bool _isThrowable;
    public bool IsThrowable => _isThrowable;

    [SerializeField]
    private EActorPropertiesType _properties;

    public EActorPropertiesType Properties
    {
        get => _properties;
        set
        {
            _properties = value;

            Color color;
            if (value == EActorPropertiesType.Flame)
            {
                color = _flameColor;
            }
            else
            {
                color = _waterColor;
            }

            GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetProperties(BaseContractInfo caller, EActorPropertiesType type)
    {
        Properties = type;
    }

    public void Die()
    {
        Destroy(gameObject);
    }


    private void Awake()
    {
        LateInit();
        Interaction.SetContractInfo(
            ActorContractInfo.Create(transform, ()=>_isDestroyed)
                .AddBehaivour<IBActorPhysics>(this)
                .AddBehaivour<IBActorLife>(this)
                .AddBehaivour<IBActorHit>(this)
                .AddBehaivour<IBActorProperties>(this)
                .AddBehaivour<IBActorThrowable>(this)
        );
        Interaction.OnContractActor += OnContractActor;
        Interaction.OnContractObject += OnContractObject;
        
        _currentHp = MaxHp;

        // Color 셋팅
        Properties = Properties;
        _effectImage.enabled = false;
    }

    private void Update()
    {
        ActorUpdate(); 
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if (info.TryGetBehaviour(out IBActorPhysics physics))
        {
            OnDetectBlock();
            OnDetectOtherActor(physics);
        }
        
        float damage = 1f;
        if (info.TryGetBehaviour(out IBActorProperties properties))
        {
            if (Properties == properties.Properties)
            {
                damage = 1f;
            }
            else
            {
                damage = 2f;
            }
        }
        if (info.TryGetBehaviour(out IBActorHit actorHit))
        {
            actorHit.DoHit(Interaction.ContractInfo, damage);
        }
    }

    public void OnContractObject(ObjectContractInfo info)
    {
    }

    [CanBeNull]
    public override Transform GetTransformOrNull()
    {
        if (_isDestroyed) return null;
        return transform;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            OnDetectBlock();
        }
    }
    public void DoHit(BaseContractInfo caller, float damage)
    {
        CurrentHP -= damage;

        if (caller.Transform.gameObject.CompareTag("Player") &&
            caller is ActorContractInfo actor &&
            actor.TryGetBehaviour(out IBActorProperties properties))
        {
            AnimatePropertiesHitEffect(properties.Properties);
        }
    }

    private Tweener _effectTweener = null;
    private bool _isDestroyed = false;
    private void OnDestroy()
    {
        _isDestroyed = true;

        if (_effectTweener != null)
        {
            _effectTweener.Kill();
            _effectTweener = null;
        }
    }
    private void AnimatePropertiesHitEffect(EActorPropertiesType type)
    {
        bool isNotNone = false;
        switch (type)
        {
            case EActorPropertiesType.None:
                isNotNone = true;
                break;
            case EActorPropertiesType.Flame:
                _effectImage.color = _flameColor;
                break;
            case EActorPropertiesType.Water:
                _effectImage.color = _waterColor;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (isNotNone) return;
        
        Color c = _effectImage.color;
        c.a = 0f;
        _effectImage.enabled = true;
        
        if(_effectTweener != null)
        {
            _effectTweener.Kill();
            _effectTweener = null;
        }
        
        _effectTweener = _effectImage.DOColor(c, 0.2f).SetLoops(5, LoopType.Yoyo).OnComplete(() =>
        {
            _effectImage.enabled = false;
            c.a = 1f;
            _effectImage.color = c;
        });
    }
}
