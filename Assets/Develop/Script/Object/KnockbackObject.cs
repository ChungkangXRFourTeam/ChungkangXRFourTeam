using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnockbackObject : MonoBehaviour, IBObjectInteractive
{
    [SerializeField] private float _knockbackForce;
    [SerializeField] private float _knockbackTime;
    [SerializeField] private float _reflectAnglel;
    [SerializeField] private float _rayLength;
    [SerializeField] private bool _debug;
    [SerializeField] private bool _isSelectiveObject;
    [SerializeField] private EActorPropertiesType _properties;

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
        
        Interaction.SetContractInfo(ObjectContractInfo.Create(transform, ()=>false)
                .AddBehaivour<IBObjectInteractive>(this)
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
        DoKnockback(info);

        if (info.TryGetBehaviour(out IBActorProperties com))
        {
            if(info.TryGetBehaviour(out IBActorHit hit))
            {
                float damage = (com.Properties == Properties ? 1f : 2f);
                hit.DoHit(Interaction.ContractInfo, damage);
            }
            
            com.SetProperties(Interaction.ContractInfo, Properties);
        }
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