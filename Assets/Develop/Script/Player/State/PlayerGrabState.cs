using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class PlayerGrabState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        var transform = blackboard.GetProperty<Transform>("out_transform");
        var grabDistance = blackboard.GetUnWrappedProperty<float>("out_grabDistance");
            
        var currentActor = PlayerCalculation.GetClickedActorOrNull(
            transform.position,
            grabDistance,
            LayerMask.GetMask("Enemy")
        );
        

        if (currentActor is null)
        {
            executor.SetNextState<DefaultPlayerState>();
            return false;
        }

        if (currentActor.TryGetContractInfo<ActorContractInfo>(out var info) && 
            info.TryGetBehaviour<IBActorThrowable>(out var throwable) &&
            throwable.IsThrowable &&
            info.TryGetBehaviour<IBActorPhysics>(out var actorPhysics))
        {
            blackboard.SetProperty("in_currentActor_physics", actorPhysics);
            executor.SetNextState<PlayerSwingState>();
        }
        else
        {
            executor.SetNextState<DefaultPlayerState>();
        }
        return false;
    }
}
