using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnockbackObject : MonoBehaviour, IBObjectInteractive, IBObjectKnockback
{
    [Header("튕기는 힘")] [SerializeField] private float _knockbackForce;

    [Header("튕기는 방향에 대한 각도")] [SerializeField]
    private float _reflectAnglel;

    [Header("속성(값이 None이면 에러 발생!! 반드시 설정)")] [SerializeField]
    private EActorPropertiesType _properties;

    [Space] [Header("시각적 디버깅 광선의 길이")] [SerializeField]
    private float _rayLength;

    [Header("시각적 디버깅 활성화 여부")] [SerializeField]
    private bool _debug;

    [Space] [SerializeField] private bool _isSelectiveObject;

    public bool IsSelectiveObject => _isSelectiveObject;
    public EActorPropertiesType Properties => _properties;
    public InteractionController Interaction { get; private set; }

    public Vector2 ReflecDirection
    {
        get
        {
            var dir = Quaternion.Euler(0f, 0f, _reflectAnglel) * Vector2.right;
            return dir;
        }
    }

    private void Awake()
    {
        Interaction = GetComponentInChildren<InteractionController>();

        Interaction.SetContractInfo(ObjectContractInfo.Create(transform, () => false)
            .AddBehaivour<IBObjectInteractive>(this)
            .AddBehaivour<IBObjectKnockback>(this)
        );
        Interaction.OnContractActor += OnContractActor;
    }

    private void Update()
    {
        Color color;
        switch (Properties)
        {
            case EActorPropertiesType.None:
                Debug.LogError("KnockbackObject�� �Ӽ��� None�� �� �����ϴ�.");
                color = Color.gray;
                break;
            case EActorPropertiesType.Flame:
                color = Color.red;
                break;
            case EActorPropertiesType.Water:
                color = Color.blue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (TryGetComponent<SpriteRenderer>(out var com))
        {
            com.color = color;
        }
    }

    public void OnContractActor(ActorContractInfo info)
    {
        if(info.Transform.TryGetComponent<Enemy>(out var e))
        {
            if (e.EnemyType == EEnemyType.Sheep) return;
        }

        DoKnockback(info);

        if (info.Transform.TryGetComponent(out PlayerController pc) &&
            !pc.IsAllowedInteraction)
            return;
        
        string key = "actor/knockbackHit";
        if (Properties == EActorPropertiesType.Flame)
        {
            key = "actor/knockbackHitOrange";
        }
        else if (Properties == EActorPropertiesType.Water)
        {
            key = "actor/knockbackHitWater";
        }

        if (info.Transform.GetComponent<BoltNut>()) return;
        EffectManager.ImmediateCommand(new EffectCommand
        {
            EffectKey = key,
            Position = info.Transform.position
        });
    }

    private void DoKnockback(ActorContractInfo info)
    {
        if (info.TryGetBehaviour(out IBActorPhysics p))
        {
            p.AddKnockback(ReflecDirection * _knockbackForce);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, ReflecDirection * _rayLength);
    }
}