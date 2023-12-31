using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XRProject.Helper;
public class DefaultPlayerState : BaseState
{
    public override void Enter(Blackboard blackboard)
    {
    }
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        if (Input.GetMouseButtonDown(1))
        {
            executor.SetNextState<PlayerGrabState>();
        }

        return false;

    }
}
