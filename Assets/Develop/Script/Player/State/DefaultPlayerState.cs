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
        blackboard.GetWrappedProperty<float>("in_coolTime", out var coolTime);
        if (coolTime <= 0f && InputManager.GetMainGameAction("Grab").triggered)
            executor.SetNextState<PlayerGrabState>();

        return true;
    }
}