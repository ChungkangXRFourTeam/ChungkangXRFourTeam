using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class PlayerShootState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetPropertyOrNull<IBActorPhysics>("in_currentActor_physics", out var currentActorPhysics);
        var lineRenderer = blackboard.GetProperty<LineRenderer>("out_lineRenderer");
        var swingForce = blackboard.GetUnWrappedProperty<float>("out_swingForce");
        var swingDir = blackboard.GetUnWrappedProperty<Vector2>("in_swingDir");

        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.016f;
        
        if(currentActorPhysics is not null)
            currentActorPhysics.AddKnockback(swingDir * swingForce);
        
        executor.SetNextState<DefaultPlayerState>();
        return false;
    }
}
