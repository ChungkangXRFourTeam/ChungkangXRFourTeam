using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRProject.Helper;

public class ActorSwingState : BaseState
{
    private float _backupGravityFactor;

    public override void Init(Blackboard blackboard)
    {
        blackboard.GetProperty<Rigidbody2D>("rigidbody", out var rigid);
        _backupGravityFactor = rigid.gravityScale;
    }

    public override void Enter(Blackboard blackboard)
    {
        blackboard.GetProperty<Rigidbody2D>("rigidbody", out var rigid);
        blackboard.GetUnWrappedProperty<Vector2>("out_knockbackVector", out var knockbackVector);

        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(knockbackVector, ForceMode2D.Impulse);
    }

    public override bool Update(Blackboard blackboard, StateExecutor executor)
    {
        blackboard.GetWrappedProperty<bool>("out_trigger_knockbackCollision", out var hit);
        blackboard.GetWrappedProperty<bool>("out_trigger_isSwingState", out var isSwingState);

        if (hit)
        {
            executor.SetNextState<ActorDefaultState>();
            return true;
        }

        if (isSwingState)
        {
            executor.SetNextState<ActorSwingState>();
            return true;
        }

        return false;
    }

    public override void Exit(Blackboard blackboard)
    {
        blackboard.GetProperty<Rigidbody2D>("rigidbody", out var rigid);
        rigid.gravityScale = _backupGravityFactor;
    }
}