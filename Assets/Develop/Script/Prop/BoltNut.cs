using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using XRProject.Boss;
using XRProject.Helper;

[RequireComponent(typeof(InteractionController),typeof(Rigidbody2D))]
public class BoltNut : MonoBehaviour, IBActorThrowable
{
    [Range(1, 10)]
    [SerializeField] private int _maxKnockbackCount;

    [SerializeField] private Vector2 _offset;
    
    private Rigidbody2D _rigid;
    private ActorPhysicsStrategy _physicsStrategy;
    private WrappedValue<bool> _isAllowedInteraction = new(true);
    private StrategyExecutor _strategyExecutor;
    private bool _swingDirty = false;
    private int _knockbackCount = 0;
    private float _gravity;
    private float _dTimer;
    public InteractionController Interaction { get; private set; }

    private void Awake()
    {
        var container = new StrategyContainer();
        container.Add(_physicsStrategy = new ActorPhysicsStrategy());
        
        var blackboard = new Blackboard();
        blackboard.AddProperty("out_rigidbody", _rigid = GetComponent<Rigidbody2D>());
        blackboard.AddProperty("out_interaction", Interaction = GetComponent<InteractionController>());
        blackboard.AddProperty("out_isAllowedInteraction", _isAllowedInteraction);
        
        _strategyExecutor = StrategyExecutor.Create(container, blackboard);
        
        Interaction.SetContractInfo(
            ActorContractInfo.Create(transform, ()=>_isdestry)
                .AddBehaivour<IBActorPhysics>(_physicsStrategy)
                .AddBehaivour<IBActorThrowable>(this)
        );

        Interaction.OnContractObject += (info) =>
        {
            if (info.Transform.GetComponent<KnockbackObject>())
            {
                if(_swingDirty)
                    _knockbackCount++;
                EffectManager.ImmediateCommand(new EffectCommand()
                {
                    EffectKey = "boss/spark",
                    Position = transform.position + (Vector3)_offset
                });
            }
        };
        Interaction.OnContractActor += info =>
        {
            if (_swingDirty == false) return;
            
            if (info.Transform.TryGetComponent<Boss>(out var boss))
            {
                boss.DoHit(Interaction.ContractInfo, 1f);
                DoDestroy();
            }
        };

        _gravity = _rigid.gravityScale;
    }

    private void DoDestroy()
    {
        _isdestry = true;
        Destroy(gameObject);
        DOTween.Kill(this);
    }

    private bool _isdestry;
    private bool fsad;
    private void Update()
    {
        if (_dTimer >= 5f && fsad == false)
        {
            fsad = true;
            GetComponent<SpriteRenderer>().DOColor(Color.gray, 0.2f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId(this)
                .OnComplete(() =>
                {
                    DoDestroy();
                });
        }
        else
        {
            _dTimer += Time.deltaTime;
        }
        
        if (_swingDirty == false) return;
        if (_knockbackCount > _maxKnockbackCount)
        {
            _rigid.gravityScale = _gravity;
            _strategyExecutor.Container.SetActive<ActorPhysicsStrategy>(false);
            return;
        }
        
        _strategyExecutor.Container.SetActive<ActorPhysicsStrategy>(true);
        _strategyExecutor.Execute();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            _physicsStrategy.OnDetectBlock();
        }
    }

    public bool IsThrowable => true;
    public void Throw(ActorContractInfo info)
    {
        _swingDirty = true;
    }
}
