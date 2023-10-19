using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using XRProject.Helper;
public class DefaultPlayerState : BaseState
{
    
    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        if(InputManager.GetMainGameAction("Grab").triggered)
            executor.SetNextState<PlayerGrabState>();
        
        return true;
    }
}
