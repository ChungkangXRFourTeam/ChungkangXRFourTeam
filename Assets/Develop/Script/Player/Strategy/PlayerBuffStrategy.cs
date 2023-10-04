using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XRProject.Helper;

[Serializable]
public class BuffInfo
{
    public float AddingDamage { get; set; }
    public float SpeedFactor { get; set; }
}

[System.Serializable]
public class PlayerBuffStrategy : IStrategy
{
    [SerializeField] private float _flameBuffAddingDamage;
    [SerializeField] private float _waterBuffMovementSpeedFactor;
    public void Init(Blackboard blackboard)
    {
    }

    public void Update(Blackboard blackboard)
    {
        blackboard.GetProperty("out_interaction", out InteractionController interaction);
        blackboard.GetProperty("out_buffInfo", out BuffInfo buffInfo);

        if (interaction.ContractInfo is ActorContractInfo actorInfo &&
            actorInfo.TryGetBehaviour(out IBActorProperties properties))
        {
            ApplyBuff(properties.Properties, buffInfo);
        }
    }

    private void ApplyBuff(EActorPropertiesType type, BuffInfo buffInfo)
    {
        switch (type)
        {
            case EActorPropertiesType.None:
                break;
            case EActorPropertiesType.Flame:
                buffInfo.AddingDamage = _flameBuffAddingDamage;
                break;
            case EActorPropertiesType.Water:
                buffInfo.SpeedFactor = _waterBuffMovementSpeedFactor;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Reset()
    {
    }
}
