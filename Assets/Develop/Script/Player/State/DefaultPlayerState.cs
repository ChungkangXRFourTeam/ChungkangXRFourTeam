using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using XRProject.Helper;
public class DefaultPlayerState : BaseState
{
    public override void Init(Blackboard blackboard)
    {
        InputManager.RegisterActionToMainGame("Grab", OnGrab, ActionType.Started);
    }
    public override void Enter(Blackboard blackboard)
    {
    }
    
    private StateExecutor _executor;
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        _executor = executor;

        return false;

    }

    void OnGrab(InputAction.CallbackContext ctx)
    {
        _executor.SetNextState<PlayerGrabState>();
    }
}
