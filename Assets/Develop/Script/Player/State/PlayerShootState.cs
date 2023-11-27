using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class PlayerShootState : BaseState
{
    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetWrappedProperty<float>("in_coolTime", out var coolTime);
        blackboard.GetProperty<PlayerSwingAttackData>("out_data", out var data);
        coolTime.Value = data.CoolTime;
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetPropertyOrNull<IBActorPhysics>("in_currentActor_physics", out var currentActorPhysics);
        blackboard.GetPropertyOrNull<IBActorPropagation>("in_currentActor_propagation", out var currentActorPropagation);
        var lineRenderer = blackboard.GetProperty<LineRenderer>("out_lineRenderer");
        var transform = blackboard.GetProperty<Transform>("out_transform");
        var swingForce = blackboard.GetUnWrappedProperty<float>("out_swingForce");
        var swingDir = blackboard.GetUnWrappedProperty<Vector2>("in_swingDir");

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        
        Time.timeScale = 1f;

        if (currentActorPhysics is not null)
        {
            if (currentActorPropagation is not null && currentActorPhysics.GetTransformOrNull() is not null)
            {
                Vector2 v = (currentActorPhysics.GetTransformOrNull().position - transform.position);
                currentActorPropagation.BeginPropagate(v.normalized);
            }

            Transform actorTransform = currentActorPhysics.GetTransformOrNull(); 
            if (actorTransform)
            {
                actorTransform.position = transform.position + Vector3.one * 1.5f;
            }
            currentActorPhysics.AddKnockback(swingDir * swingForce);

            if (currentActorPhysics.Interaction.TryGetContractInfo(out ActorContractInfo actorContractInfo) &&
                actorContractInfo.TryGetBehaviour(out IBActorAttackable attackable))
            {
                attackable.IsAttackable = true;
            }
            
            if (currentActorPhysics.Interaction.TryGetContractInfo(out ActorContractInfo actorContractInfo2) &&
                actorContractInfo2.TryGetBehaviour(out IBActorThrowable throwable))
            {
                blackboard.GetProperty("out_interaction", out InteractionController inter);
                throwable.Throw(inter.ContractInfo as ActorContractInfo);
            }

        }
        
        executor.SetNextState<DefaultPlayerState>();
        return false;
    }
}
