using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using XRProject.Helper;

public class PlayerController : MonoBehaviour, IBActorProperties, IBActorHit, IBActorLife
{
    [SerializeField] private PlayerMoveStrategy _moveStrategy;
    [SerializeField] private PlayerPhysicsStrategy _physicsStrategy;
    [SerializeField] private PlayerMeleeAttackStrategy _meleeAttackStrategy;
    [SerializeField] private PlayerSwingAttackStrategy _swingAttackStrategy;
    [SerializeField] private InteractionController _interaction;
    [SerializeField] private EActorPropertiesType _properties;
    [SerializeField] private WrappedValue<int> _propertiesCount;
    [SerializeField] private float _hp;
    
    
    private StateExecutor _stateExecutor;
    private float _currentHp;
    private WrappedValue<bool> _isAllowedInteraction = new(false);

    public InteractionController Interaction => _interaction;
    public EActorPropertiesType Properties => _properties;

    public bool IsAllowedInteraction
    {
        get => _isAllowedInteraction;
        set => _isAllowedInteraction.Value = value;
    }
    
    public int RemainingPropertie => _propertiesCount;
    
    
    public bool IsDestroyed { get; private set; }

    public event Action<IBActorLife, float, float> ChangedHp;
    public float MaxHp => _hp;
    
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
    

    public void Die()
    {
        IsDestroyed = true;
        Destroy(gameObject);
    }

    public void DoHit(BaseContractInfo caller, float damage)
    {
        if (caller.Transform.gameObject.CompareTag("Boss"))
        {
            CurrentHP -= damage;
        }
    }

    private void Awake()
    {
        CurrentHP = MaxHp;
        
        // interaction initializing..
        Interaction.SetContractInfo(ActorContractInfo.Create(transform, ()=>false)
            .AddBehaivour<IBActorPhysics>(_physicsStrategy)
            .AddBehaivour<IBActorProperties>(this)
            .AddBehaivour<IBActorHit>(this)
            .AddBehaivour<IBActorLife>(this)
        );
        
        // Strategy initializing..
        var strategyContainer = new StrategyContainer();
        var strategyBlackboard = new Blackboard();
        
        strategyBlackboard
            .AddProperty("out_transform", transform)
            .AddProperty("out_interaction", _interaction)
            .AddProperty("out_remaingProperties", _propertiesCount)
            .AddProperty("out_isAllowedInteraction", _isAllowedInteraction)
            ;

        strategyContainer
            .Add(_moveStrategy)
            .Add(_physicsStrategy)
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
            ;
        
        _stateExecutor = StateExecutor.Create(stateContainer, fsmBlackboard);
        
    }

    private void Update()
    {
        _stateExecutor.Execute();

        IsAllowedInteraction = Input.GetMouseButton(0);
    }
    
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.TryGetComponent<IBObjectInteractive>(out var com))
        {
            if(com.IsSelectiveObject && !IsAllowedInteraction)
                _physicsStrategy.OnDetectBlock();
        }
        else
        {
            _physicsStrategy.OnDetectBlock();
        }
    }

    public void SetProperties(BaseContractInfo caller, EActorPropertiesType type)
    {
        if(type != EActorPropertiesType.None)
            _propertiesCount.Value = 10;

        _properties = type;
    }
}
