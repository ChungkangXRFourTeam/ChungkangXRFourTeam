using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class ActorDefaultState : BaseState
{
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetWrappedProperty<bool>("out_trigger_isSwingState", out var isSwingState);
        
        if (isSwingState)
        {
            executor.SetNextState<ActorSwingState>();
            return true;
        }

        return false;
    }
}
