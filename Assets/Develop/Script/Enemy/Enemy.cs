using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using XRProject.Helper;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IBActorLife, IBActorProperties, IBActorHit, IBActorThrowable
{
    [SerializeField] private Color _flameColor;
    [SerializeField] private Color _waterColor;
    [SerializeField] private Image _effectImage;
    [SerializeField] private float _hp;
    [SerializeField] private float _speed;
    [SerializeField] private Collider2D _body;

    private ActorPhysicsStrategy _physicsStrategy = new();
    
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

    [SerializeField] private Rigidbody2D _rigid;
    public Rigidbody2D Rigid => _rigid;
    [SerializeField] private InteractionController _interaction;

    public InteractionController Interaction => _interaction;

    private StateExecutor _executor;
    
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
        
        // Strategy initializing..
        var strategyContainer = new StrategyContainer();
        var strategyBlackboard = new Blackboard();

        strategyBlackboard
            .AddProperty("out_transform", transform)
            .AddProperty("out_rigidbody", Rigid)
            .AddProperty("out_interaction", Interaction)
            .AddProperty("out_isAllowedInteraction", new WrappedValue<bool>(true))
            ;

        strategyContainer
            .Add(_physicsStrategy)
            .Add(new EnemyPropagationStrategy())
            ;
        var strategyExecutor = StrategyExecutor.Create(strategyContainer, strategyBlackboard);
        
        // FSM 셋팅
        Blackboard blackboard = new Blackboard();
        StateContainer container = new StateContainer();

        container
            .AddState<EnemyDefaultState>()
            .SetInitialState<EnemyDefaultState>()
            ;

        blackboard
            .AddProperty("out_transform", transform)
            .AddProperty("out_strategyExecutor", strategyExecutor)
            .AddProperty("out_interaction", Interaction)
            ;

        _executor = StateExecutor.Create(container, blackboard);
        
        
        Interaction.SetContractInfo(
            ActorContractInfo.Create(transform, ()=>_isDestroyed)
                .AddBehaivour<IBActorPhysics>(_physicsStrategy)
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
                _goLeft = Random.value >= 0.5f;
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
    
    private bool IsSwingState => _physicsStrategy.IsSwingState;

    private void Update()
    {
        _executor.Execute();
        
        _body.isTrigger = !IsSwingState;
        // 중간시연용 코드
        if (IsSwingState || !_checkPoint)
        {
            return;
        }

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
                    Position = Vector3.Lerp(transform.position, info.Transform.position, 0.5f)
                });
            }
        }
        
        if (info.TryGetBehaviour(out IBActorPhysics physics))
        {
            _physicsStrategy.OnDetectBlock();
            _physicsStrategy.OnDetectOtherActor(physics);
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
        if (info.TryGetBehaviour(out IBActorHit actorHit) && IsSwingState)
        {
            actorHit.DoHit(Interaction.ContractInfo, damage);
            DoHit(info, damage);
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
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _physicsStrategy.OnDetectBlock();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PatrollSpace>())
        {
            _checkPoint = false;
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
