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
    /* seralizedFields */
    [Header("editable")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private bool _isThrowable;
    [SerializeField] private EActorPropertiesType _properties;
    [Header("don't edit this")]
    [SerializeField] private Image _effectImage;
    [SerializeField] private Collider2D _body;
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private InteractionController _interaction;

    /* fields */
    private ActorPhysicsStrategy _physicsStrategy = new();
    private StateExecutor _executor;
    private PropagationInfo _propagationInfo;
    private Tweener _effectTweener = null;
    private bool _isDestroyed = false;
    
    /* properties */
    public event System.Action<IBActorLife, float, float> ChangedHp;
    public float MaxHp => _data.Hp;
    
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

    public bool IsThrowable => _isThrowable;


    public EActorPropertiesType Properties
    {
        get => _properties;
        set
        {
            _properties = value;
        }
    }

    public Rigidbody2D Rigid => _rigid;

    public InteractionController Interaction => _interaction;
    private bool IsSwingState => _physicsStrategy.IsSwingState;
    public string test;

    /* unity functions */
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
            .AddProperty("out_rigidbody", Rigid)
            .AddProperty("out_strategyExecutor", strategyExecutor)
            .AddProperty("out_interaction", Interaction)
            .AddProperty("test", "")
            
            .AddProperty("out_enemyData", _data)
            .AddProperty("out_patrollPoints", new WrappedValue<(Vector2, Vector2)>())
            .AddProperty("out_isEnteredPatrollSpace", new WrappedValue<bool>(false))
            .AddProperty("out_propagationInfo", _propagationInfo = new PropagationInfo(Interaction))
            .AddProperty("out_enemyBody", _body)
            
            .AddProperty("in_isMoving", new WrappedValue<bool>(false))
            .AddProperty("in_isStop", new WrappedValue<bool>(false))
            .AddProperty("in_isSleep", new WrappedValue<bool>(false))
            
            
            .AddProperty("out_trigger_isPropagating",  new WrappedTriggerValue())
            ;

        _executor = StateExecutor.Create(container, blackboard);
        
        
        Interaction.SetContractInfo(
            ActorContractInfo.Create(transform, ()=>_isDestroyed)
                .AddBehaivour<IBActorPropagation>(_propagationInfo)
                .AddBehaivour<IBActorPhysics>(_physicsStrategy)
                .AddBehaivour<IBActorLife>(this)
                .AddBehaivour<IBActorHit>(this)
                .AddBehaivour<IBActorProperties>(this)
                .AddBehaivour<IBActorThrowable>(this)
                .AddBehaivour<IBEnemyState>(container.GetState<EnemyDefaultState>())
        );
        Interaction.OnContractActor += OnContractActor;
        
        _currentHp = MaxHp;

        // Color 셋팅
        Properties = Properties;
        _effectImage.enabled = false;
    }
    

    private void Update()
    {
        test = _executor.Blackboard.GetProperty<string>("test");
        _executor.Execute();
        //_body.isTrigger = !IsSwingState;
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
            _executor.Blackboard.GetWrappedProperty<bool>("out_isEnteredPatrollSpace").Value = true;
        }
    }

    /* methods */
    public void SetProperties(BaseContractInfo caller, EActorPropertiesType type)
    {
        Properties = type;
    }

    public void Die()
    {
        _executor.Release();
        _isDestroyed = true;
        DOTween.Kill(this);

        if (_effectTweener != null)
        {
            _effectTweener.Kill();
            _effectTweener = null;
        }
        Destroy(gameObject);
    }

    private void OnContractActor(ActorContractInfo info)
    {
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
    private void AnimatePropertiesHitEffect(EActorPropertiesType type)
    {
        bool isNotNone = false;
        switch (type)
        {
            case EActorPropertiesType.None:
                isNotNone = true;
                break;
            case EActorPropertiesType.Flame:
                _effectImage.color = _data.FlameColor;
                break;
            case EActorPropertiesType.Water:
                _effectImage.color = _data.WaterColor;
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
