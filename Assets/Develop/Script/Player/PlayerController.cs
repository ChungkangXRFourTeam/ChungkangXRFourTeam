using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using XRProject.Helper;

public class PlayerController : MonoBehaviour, IBActorProperties, IBActorHit, IBActorLife
{
    [Header("don't edit this")]
    /* components */
    [SerializeField] private PlayerFoot _foot;
    [SerializeField] private PlayerFoot _leftSide;
    [SerializeField] private PlayerFoot _rightside;
    [SerializeField] private Transform _meleeHand;
    [SerializeField] private LineRenderer _traceLineRenderer;
    [SerializeField] private InteractionController _interaction;
    [SerializeField] private PlayerAnimationController _aniController;
    [SerializeField] private ParticleSystem _vignettte;
    
    [Header("can edit this, it is behaviour related data of player")]
    /* datas */
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerMoveData _moveData;
    [SerializeField] private PlayerMeleeAttackData _meleeAttackData;
    [SerializeField] private PlayerSwingAttackData _swingAttackData;
    [SerializeField] private PlayerBuffData _buffData;

    /* strategies */
    private PlayerMoveStrategy _moveStrategy = new();
    private ActorPhysicsStrategy _physicsStrategy = new();
    private PlayerMeleeAttackStrategy _meleeAttackStrategy = new();
    private PlayerSwingAttackStrategy _swingAttackStrategy = new();
    private PlayerBuffStrategy _buffStrategy = new();
    
    /* fields */
    private BuffInfo _buffInfo = new BuffInfo();
    private StateExecutor _stateExecutor;
    private float _currentHp;
    private WrappedValue<int> _propertiesCount = new WrappedValue<int>(0);
    private WrappedValue<bool> _isAllowedInteraction = new(false);
    [SerializeField]
    private EActorPropertiesType _properties = EActorPropertiesType.None;
    
    /* properties */
    public PlayerMeleeAttackData MeleeAttackData => _meleeAttackData;
    public InteractionController Interaction => _interaction;
    public EActorPropertiesType Properties => _properties;
    public bool AniKnockback { get; set; }
    [SerializeField]
    private WrappedValue<bool> _grabState = new(false);
    public bool GrabState => _grabState.Value;
    public bool IsAllowedInteraction
    {
        get => _isAllowedInteraction;
        set => _isAllowedInteraction.Value = value;
    }

    public int RemainingPropertie => _propertiesCount;
    public float SwingCoolTime => _swingAttackStrategy.CoolTime;
    public float MaxSwingCoolTime => _swingAttackData.CoolTime;


    public bool IsDestroyed { get; private set; }
    public float MaxHp => _playerData.Hp;

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

    /* events */
    public event Action<IBActorLife, float, float> ChangedHp;
    public event Action<EActorPropertiesType> ChangedProperties;
    
    /* unity functions */
    private void Awake()
    {
        CurrentHP = MaxHp;

        InputManager.RegisterActionToMainGame("BoundMode", OnBoundMode, ActionType.Started);
        InputManager.RegisterActionToMainGame("BoundMode", ExitBoundMode, ActionType.Canceled);

        var isGrounded = new WrappedValue<bool>();
        var isLeftSide = new WrappedValue<bool>();
        var isRightSide = new WrappedValue<bool>();
        _foot.OnChangeIsGround += (x) => isGrounded.Value = x;
        _leftSide.OnChangeIsGround += (x) => isLeftSide.Value = x;
        _rightside.OnChangeIsGround += (x) => isRightSide.Value = x;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();

        // interaction initializing..
        Interaction.SetContractInfo(ActorContractInfo.Create(transform, () => false)
            .AddBehaivour<IBActorPhysics>(_physicsStrategy)
            .AddBehaivour<IBActorProperties>(this)
            .AddBehaivour<IBActorHit>(this)
            .AddBehaivour<IBActorLife>(this)
        );

        Interaction.OnContractObject += x =>
        {
            if (x.TryGetBehaviour(out IBObjectKnockback knockback))
            {
                SetProperties(x,knockback.Properties);
            }
            if (x.TryGetBehaviour(out IBObjectInteractive interactive))
            {
                AniKnockback = true;
            }
            else
            {
                AniKnockback = false;
            }
        };

        // Strategy initializing..
        var strategyContainer = new StrategyContainer();
        var strategyBlackboard = new Blackboard();

        strategyBlackboard
            .AddProperty("out_transform", transform)
            .AddProperty("out_rigidbody", rigid)
            .AddProperty("out_interaction", _interaction)
            .AddProperty("out_remaingProperties", _propertiesCount)
            .AddProperty("out_isAllowedInteraction", _isAllowedInteraction)
            .AddProperty("out_buffInfo", _buffInfo)
            .AddProperty("out_isGrounded", isGrounded)
            .AddProperty("out_isLeftSide", isLeftSide)
            .AddProperty("out_isRightSide", isRightSide)
            .AddProperty("out_moveData", _moveData)
            .AddProperty("out_meleeAttackData", _meleeAttackData)
            .AddProperty("out_swingAttackData", _swingAttackData)
            .AddProperty("out_buffData", _buffData)
            .AddProperty("out_meleeHand", _meleeHand)
            .AddProperty("out_traceLineRenderer", _traceLineRenderer)
            .AddProperty("out_aniController", _aniController)
            .AddProperty("in_isGrabState", _grabState)
            ;

        strategyContainer
            .Add(_moveStrategy)
            .Add(_physicsStrategy)
            .Add(_buffStrategy)
            .Add(_meleeAttackStrategy)
            .Add(_swingAttackStrategy)
            ;

        var strategyExecutor = StrategyExecutor.Create(strategyContainer, strategyBlackboard);

        // FSM initializing..
        var stateContainer = new StateContainer();
        var fsmBlackboard = new Blackboard();

        stateContainer
            .AddState<PlayerDefaultState>()
            .SetInitialState<PlayerDefaultState>()
            ;

        fsmBlackboard
            .AddProperty("out_strategyExecutor", strategyExecutor)
            .AddProperty("out_aniController", _aniController)
            .AddProperty("out_interaction", _interaction)
            ;

        _stateExecutor = StateExecutor.Create(stateContainer, fsmBlackboard);

        ChangedHp += DoVignette;
        
        _vignettte.Stop();
    }

    private void DoVignette(IBActorLife life, float bh, float ch)
    {
        if (ch <= 3f)
        {
            if(_vignettte.isPlaying == false)
                _vignettte.Play();
        }
        else
        {
            _vignettte.Stop();
        }
    }
    private void Update()
    {
        if (IsDestroyed) return;
        
        _stateExecutor.Execute();
        
        /*
        // 스프라이트 반전 코드. 이후 애니메이션이 나오면 삭제
        var x = InputManager.GetMainGameAction("Move")?.ReadValue<Vector2>().x ?? 0f;
        bool flipX = x < 0f;
        if (TryGetComponent<SpriteRenderer>(out var renderer))
            renderer.flipX = flipX;
            */
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.TryGetComponent<IBObjectInteractive>(out var com))
        {
            if (com.IsSelectiveObject && !IsAllowedInteraction)
            {
                _physicsStrategy.OnDetectBlock();
            }
        }
        else
        {
            _physicsStrategy.OnDetectBlock();
        }
    }

    
    /* functions */
    public void Die()
    {
        if (IsDestroyed) return;

        IsDestroyed = true;
        _vignettte.Stop();
        
        _aniController.SetState(new PAniState()
        {
            State = EPCAniState.Death
        });

        DOTween.Kill(_hitKey);
        GetComponent<SpriteRenderer>().color = Color.white;
        TalkingEventManager.Instance.InvokeCurrentEvent(new DeathEvent()).Forget();
    }

    private void OnEndDeath()
    {
    }

    private object _hitKey = new();
    public void DoHit(BaseContractInfo caller, float damage)
    {
        CurrentHP -= damage;
        
        //EffectManager.ImmediateCommand(new EffectCommand
        //{
        //    EffectKey = "actor/knockbackHit",
        //    Position = transform.position
        //});

        if (CurrentHP <= 0f) return;
        DOTween.Kill(_hitKey);
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<SpriteRenderer>()?.DOColor(Color.gray, 0.125f).SetLoops(8, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId(_hitKey);
    }

    public void SetPropertiesCount(BaseContractInfo caller, int count)
    {
        _propertiesCount.Value = count;
        ChangedProperties?.Invoke(Properties);
    }
    public void SetProperties(BaseContractInfo caller, EActorPropertiesType type)
    {
        if (type != EActorPropertiesType.None)
            _propertiesCount.Value = 10;

        ChangedProperties?.Invoke(type);
        
        _properties = type;

        DOTween.Kill(this, false);
        Sequence s = DOTween.Sequence();
        s.SetDelay(_playerData.RemoveDelayWithProperties).OnComplete(() =>
        {
            _properties = EActorPropertiesType.None;
            _propertiesCount.Value = 0;
        }).SetId(this);
    }

    void OnBoundMode(InputAction.CallbackContext ctx)
    {
        IsAllowedInteraction = true;
    }

    void ExitBoundMode(InputAction.CallbackContext ctx)
    {
        IsAllowedInteraction = false;
    }
}