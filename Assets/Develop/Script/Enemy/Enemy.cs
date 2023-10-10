using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : ActorPhysics, IBActorLife, IBActorProperties, IBActorHit, IBActorThrowable
{
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Image _effectImage;
    [SerializeField] private float _hp;
    [SerializeField] private float _speed;
    [SerializeField] private Collider2D _body;

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
        
        // 중간 시연용 코드
        Interaction.OnContractObject += (info) =>
        {
            if (info.TryGetBehaviour(out IBObjectPatrollSpace patrollSpace))
            {
                _leftPoint = patrollSpace.LeftPoint;
                _rightPoint = patrollSpace.RightPoint;
                _goLeft = !_goLeft;
                _checkPoint = true;
            }
        };
        
        _currentHp = MaxHp;

        // Color 셋팅
        Properties = Properties;
        _effectImage.enabled = false;
    }
    
    // 중간 시연용 코드
    private bool _checkPoint;
    private Vector2 _leftPoint;
    private Vector2 _rightPoint;
    private Vector2 TargetPoint => _goLeft ? _leftPoint : _rightPoint;
    private bool _goLeft;

    private void Update()
    {
        ActorUpdate();
        
        _body.isTrigger = !IsSwingState;
        // 중간시연용 코드
        if (IsSwingState || !_checkPoint)
        {
            return;
        }
        print(IsSwingState);

        if (Mathf.Abs(TargetPoint.x - transform.position.x) <= 0.1f + 1.3f * 0.5f)
        {
            _goLeft = !_goLeft;
        }

        var dir = TargetPoint - (Vector2)transform.position;
        dir = dir.normalized;
        dir = Vector3.Project(dir, Vector3.right).normalized;

        transform.position += (Vector3)dir * (_speed * Time.deltaTime);
    }

    private void OnContractActor(ActorContractInfo info)
    {
        if (info.Transform.GetComponent<Enemy>())
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => _body.isTrigger = false).SetDelay(0.05f)
                .AppendCallback(() => _body.isTrigger = true).SetId(this);

            if (IsSwingState)
            {
                EffectManager.ImmediateCommand(new EffectCommand()
                {
                    EffectKey = "actor/enemyHit",
                    Position = transform.position
                });
            }
        }
        
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
        if (info.TryGetBehaviour(out IBObjectInteractive interactive) &&
            info.Transform.gameObject.CompareTag("KnockbackObject"))
        {
            EffectManager.ImmediateCommand(new EffectCommand()
            {
                EffectKey = "actor/knockbackHit",
                Position = transform.position
            });
        }
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
            caller is ActorContractInfo actor)
        {
            if(actor.TryGetBehaviour(out IBActorProperties properties))
            {
                AnimatePropertiesHitEffect(properties.Properties);
            }
            
            EffectManager.ImmediateCommand( new EffectCommand()
            {
                EffectKey = "actor/enemyHit",
                Position = transform.position
            });
        }
    }

    private Tweener _effectTweener = null;
    private bool _isDestroyed = false;
    private void OnDestroy()
    {
        _isDestroyed = true;
        DOTween.Kill(this);

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
