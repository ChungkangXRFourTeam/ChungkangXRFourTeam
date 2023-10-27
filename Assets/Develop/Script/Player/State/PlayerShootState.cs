using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class 
    PlayerShootState : BaseState
{
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

            if (currentActorPhysics.GetTransformOrNull())
            {
                currentActorPhysics.GetTransformOrNull().position = transform.position + Vector3.one * 1.5f;
            }
            currentActorPhysics.AddKnockback(swingDir * swingForce);
        }
        
        executor.SetNextState<DefaultPlayerState>();
        return false;
    }
}
