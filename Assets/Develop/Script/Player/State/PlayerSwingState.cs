using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class PlayerSwingState : BaseState
{
    public override void Enter(Blackboard blackboard)
    {
        if (!blackboard.TryGetProperty<IBActorPhysics>("in_currentActor_physics", out var currentActor))
        {
            throw new Exception();
        }
        
        var timeScale = blackboard.GetUnWrappedProperty<float>("out_timeScale");
        
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.016f;
        
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetPropertyOrNull<IBActorPhysics>("in_currentActor_physics", out var currentActorPhysics);

        
        var lineRenderer = blackboard.GetProperty<LineRenderer>("out_lineRenderer");
        var transform = blackboard.GetProperty<Transform>("out_transform");
        var minmumCloseDistance = blackboard.GetUnWrappedProperty<float>("out_minmumCloseDistance");
        

        var actorTransform = currentActorPhysics.GetTransformOrNull();
        if (actorTransform == null)
        {
            lineRenderer.enabled = false;
            executor.SetNextState<DefaultPlayerState>();
            return false;
        }
        
        var dir = ((Vector2)actorTransform.position - (Vector2)transform.position).normalized;

        var actorPos = (dir * minmumCloseDistance) + (Vector2)transform.position;
        actorTransform.position = actorPos;

        var swingDir = PlayerCalculation.GetSwingDirection(Camera.main, actorTransform.position);
        var points = PlayerCalculation.GetReflectionPoints(actorTransform.position, swingDir);

        lineRenderer.enabled = true;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        //Debug.DrawRay(actorPos, swingDir * 5f);

        if (!Input.GetMouseButtonDown(0)) return false;
        blackboard.SetWrappedProperty("in_swingDir", swingDir);
        executor.SetNextState<PlayerShootState>();

        return false;
    }
    
    public override void Exit(Blackboard blackboard)
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.016f;
    }
}
