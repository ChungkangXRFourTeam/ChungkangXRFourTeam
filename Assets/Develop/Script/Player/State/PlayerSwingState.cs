using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
        
        
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        bool rtv = Swing(blackboard, executor);
        return rtv;
    }

    private bool Swing(Blackboard blackboard, StateExecutor executor)
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
        var actorBoxCollider = actorTransform.GetComponent<BoxCollider2D>();

        if (!actorBoxCollider) return false;
        var colSize = new Vector2(
            actorBoxCollider.size.x * actorTransform.lossyScale.x,
            actorBoxCollider.size.y * actorTransform.lossyScale.y
        );

        var actorPos = (dir * minmumCloseDistance) + (Vector2)transform.position;
        actorTransform.position = actorPos;
        
        var swingDir = PlayerCalculation.GetSwingDirection(Camera.main, actorTransform.position);
        var points = PlayerCalculation.GetReflectionPoints(
            actorTransform.position, 
            swingDir,
            colSize,
            actorBoxCollider.offset,
            0f
        );
        
        lineRenderer.enabled = true;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        if (!InputManager.GetMainGameAction("Swing").triggered) return false;
        blackboard.SetWrappedProperty("in_swingDir", swingDir);
        executor.SetNextState<PlayerShootState>();
        return false;
    }
    
    public override void Exit(Blackboard blackboard)
    {
        Time.timeScale = 1f;
    }
}
