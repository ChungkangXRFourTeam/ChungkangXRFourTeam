using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using XRProject.Helper;

public enum EEnemyType
{
    Hedgehog,
    Sheep
}
public class Enemy : MonoBehaviour, IBActorLife, IBActorProperties, IBActorHit, IBActorThrowable, IBActorAttackable
{
    /* seralizedFields */
    [Header("editable")]
    [SerializeField] private EnemyData _data;
    [SerializeField] private bool _isThrowable;
    [SerializeField] private EActorPropertiesType _properties;
    [SerializeField] private EEnemyType _enemyType;

    [Header("don't edit this")] 
    [SerializeField] private AnimationBodyChanger _bodyChanger;
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
    private WrappedTriggerValue _isAttackEnded = new();
    private WrappedValue<bool> _isSpineAttackEvent=new(); 
    private WrappedValue<bool> _isSpineHitEvent=new(); 
    
    /* properties */
    private WrappedValue<bool> _isCaught = new();
    public bool IsAttackable 
    {
        get => !_isCaught.Value;
        set => _isCaught.Value = !value;
    }

    
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
    public void Throw(ActorContractInfo info)
    {
        
    }

    public void SetThrowable(bool value) => _isThrowable = value;

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
    public WrappedValue<float> testMixDuration;

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
            .AddProperty("out_skeletonAnimation", _bodyChanger.GetBodyGetComponentOrNull<SkeletonAnimation>("default"))
            .AddProperty("out_bodyChanger", _bodyChanger)
            .AddProperty("out_isSpineAttackEvent", _isSpineAttackEvent)
            .AddProperty("out_isSpineHitEvent", _isSpineHitEvent)
            
            .AddProperty("out_enemyData", _data)
            .AddProperty("out_patrollPoints", new WrappedValue<(Vector2, Vector2)>())
            .AddProperty("out_isEnteredPatrollSpace", new WrappedValue<bool>(false))
            .AddProperty("out_propagationInfo", _propagationInfo = new PropagationInfo(Interaction))
            .AddProperty("out_enemyBody", _body)
            .AddProperty("out_enemyType", new WrappedValue<EEnemyType>(_enemyType))
            
            .AddProperty("in_isMoving", new WrappedValue<bool>(false))
            .AddProperty("in_isStop", new WrappedValue<bool>(false))
            .AddProperty("in_isSleep", new WrappedValue<bool>(false))
            .AddProperty("out_isCaught", _isCaught)
            
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
                .AddBehaivour<IBActorAttackable>(this)
                .AddBehaivour<IBEnemyState>(container.GetState<EnemyDefaultState>())
        );
        
        Interaction.OnContractActor += OnContractActor;
        Interaction.OnContractObject += OnContractObject;
        
        _currentHp = MaxHp;

        // Color 셋팅
        Properties = Properties;
        _effectImage.enabled = false;

        if (_enemyType == EEnemyType.Sheep)
        {
            var shotBody = _bodyChanger.GetBodyGetComponentOrNull<SkeletonAnimation>("shot");
            var defaultBody = _bodyChanger.GetBodyGetComponentOrNull<SkeletonAnimation>("default");

            if (shotBody && defaultBody)
            {
                shotBody.AnimationState.Complete += x =>
                {
                    _bodyChanger.Change("default");
                };
                shotBody.AnimationState.Start += x =>
                {
                    _bodyChanger.Change("shot");
                };

                defaultBody.state.Event += (t, e) =>
                {
                    if (e.Data.Name == "attack")
                    {
                        _isSpineAttackEvent.Value = true;
                    }
                };
            }
        }
        else if (_enemyType == EEnemyType.Hedgehog)
        {
            var shotBody = _bodyChanger.GetBodyGetComponentOrNull<SkeletonAnimation>("shot");
            var attackBody = _bodyChanger.GetBodyGetComponentOrNull<SkeletonAnimation>("default");

            if (shotBody)
            {
                shotBody.AnimationState.Complete += x =>
                {
                    _isSpineHitEvent.Value = false;
                    _bodyChanger.Change("move");
                };
                shotBody.AnimationState.Start += x =>
                {
                    _isSpineHitEvent.Value = true;
                    _bodyChanger.Change("shot");
                };
            }
        }
        
    }
    

    private void Update()
    {
        if (_isDestroyed) return;
        
        _executor.Execute();
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

        if (_enemyType == EEnemyType.Sheep)
        {
            var ani = _bodyChanger
                .Change("death")
                .GetBodyGetComponentOrNull<SkeletonAnimation>("death");
            
            ani!.AnimationState.Complete += x =>
            {
                Destroy(gameObject);
            };
            ani!.AnimationState.SetAnimation(0, "death", false);
        }
        else
        {
            var ani = _bodyChanger
                .Change("death")
                .GetBodyGetComponentOrNull<SkeletonAnimation>("death");
            
            ani!.AnimationState.Complete += x =>
            {
                Destroy(gameObject);
            };
            ani!.AnimationState.SetAnimation(0, "animation", false);
        }
    }

    private void OnContractObject(ObjectContractInfo info)
    {
        if (info.TryGetBehaviour(out IBObjectKnockback knockback))
        {
            SetProperties(info, knockback.Properties);
        }    
    }
    
    private void OnContractActor(ActorContractInfo info)
    {
        if (info.Transform.gameObject.CompareTag("Boss"))
        {
            if(info.TryGetBehaviour(out IBActorHit bossHit) &&
                _physicsStrategy.IsSwingState)
            {
                bossHit.DoHit(Interaction.ContractInfo,1f);
            }
            return;
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

    public void DoHit(BaseContractInfo caller, float damage)
    {
        if (CurrentHP <= 0) return;
        
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

            if ((_enemyType == EEnemyType.Sheep || _enemyType == EEnemyType.Hedgehog) && CurrentHP > 0)
            {
                _bodyChanger
                    .Change("shot")
                    .GetBodyGetComponentOrNull<SkeletonAnimation>("shot")?
                    .AnimationState.SetAnimation(0, "shot", false)
                    ;
            }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.DetectionDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _data.AttackDistance);
    }
}
